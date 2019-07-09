using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Linq;

namespace ObjectDetector
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel()
        {
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
            OpenCustomVisionCommand = new Command(async () => await Xamarin.Essentials.Launcher.OpenAsync("https://customvision.ai"));
            OkCommand = new Command(async () => await Ok(), () => !string.IsNullOrWhiteSpace(PredictionKey) &&
                                                                  !string.IsNullOrWhiteSpace(ProjectId) &&
                                                                  !string.IsNullOrWhiteSpace(PublishName) &&
                                                                  !string.IsNullOrWhiteSpace(Endpoint) &&
                                                                  Guid.TryParse(ProjectId, out var g));
            ConfigureCanCancelCommand = new Command(async () => CanCancel = !string.IsNullOrWhiteSpace(await KeyService.GetPredictionKey()) && !string.IsNullOrWhiteSpace(await KeyService.GetProjectId()));
            CancelCommand = new Command(async () => await Application.Current.MainPage.Navigation.PopModalAsync());
            CapturePredictionKeyCommand = new Command(async () => await CapturePredictionKey());
            CaptureProjectIdCommand = new Command(async () => await CaptureProjectId());
            CapturePublishNameCommand = new Command(async () => await CapturePublishName());
            CaptureEndpointCommand = new Command(async () => await CaptureEndpoint());
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void

            IsEnabled = false;
        }

        public async Task SetUp()
        {
            if (string.IsNullOrWhiteSpace(PredictionKey) &&
                string.IsNullOrWhiteSpace(ProjectId) &&
                string.IsNullOrWhiteSpace(PublishName))
            {
                try
                {
                    PredictionKey = await KeyService.GetPredictionKey();
                    ProjectId = await KeyService.GetProjectId();
                    PublishName = await KeyService.GetPublishName();
                }
                finally
                {
                    IsEnabled = true;
                }
            }
        }

        async Task CapturePublishName()
        {
            Analytics.TrackEvent("Capture publish name");

            IsEnabled = false;
            try
            {
                var words = await GetLines();
                var name = words.FirstOrDefault(w => w.ToLower().StartsWith("published as:"));
                if (name == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Couldn't detect Project Id", "OK");
                }
                else
                {
                    var colon = name.IndexOf(':');
                    var publishPart = name.Substring(colon + 1).Trim();
                    if (string.IsNullOrWhiteSpace(publishPart))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Couldn't detect Project Id", "OK");
                    }
                    else
                    {
                        PublishName = publishPart;
                    }
                }
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Couldn't detect Project Id", "OK");
            }
            finally
            {
                IsEnabled = true;
            }
        }

        async Task CaptureProjectId()
        {
            Analytics.TrackEvent("Capture Project Id");

            IsEnabled = false;
            try
            {
                var key = await GetValue(l => l.ToLower().StartsWith("project") && l.ToLower().EndsWith("id"));
                if (string.IsNullOrWhiteSpace(key))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Couldn't detect Project Id", "OK");
                }
                else
                {
                    ProjectId = key;
                }
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Couldn't detect Project Id", "OK");
            }
            finally
            {
                IsEnabled = true;
            }
        }

        async Task CaptureEndpoint()
        {
            Analytics.TrackEvent("Capture Endpoint");

            IsEnabled = false;
            try
            {
                var foundEndpoint = await GetValue(l => l.ToLower().StartsWith("prediction") && l.ToLower().EndsWith("endpoint:"));
                if (string.IsNullOrWhiteSpace(foundEndpoint))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Couldn't detect Prediction Endpoint", "OK");
                }
                else
                {
                    var endPart = "api.cognitive.microsoft.com/";
                    var split = foundEndpoint.ToLower().IndexOf(endPart) + endPart.Length;

                    Endpoint = foundEndpoint.Substring(0, split);
                }
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Couldn't detect Prediction Endpoint", "OK");
            }
            finally
            {
                IsEnabled = true;
            }
        }

        async Task CapturePredictionKey()
        {
            Analytics.TrackEvent("Capture Prediction Key");

            IsEnabled = false;
            try
            {
                var key = await GetValue(l => l.ToLower().StartsWith("prediction") && l.ToLower().EndsWith("key:"));
                if (string.IsNullOrWhiteSpace(key))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Couldn't detect Prediction Key", "OK");
                }
                else
                {
                    PredictionKey = key;
                }
            }
            catch (Exception ex)
            {
                Analytics.TrackEvent("Couldn't detect Prediction Key", new Dictionary<string, string> { { "Error", "Couldn't detect Prediction Key" } });
                Crashes.TrackError(ex, new Dictionary<string, string> { { "Action", "Detect Prediction Key" } });
                await Application.Current.MainPage.DisplayAlert("Error", "Couldn't detect Prediction Key", "OK");
            }
            finally
            {
                IsEnabled = true;
            }
        }

        async Task<List<string>> GetLines()
        {
            var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions { PhotoSize = PhotoSize.Full });
            if (photo != null)
            {
                var computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(AppConstants.OcrKey))
                {
                    Endpoint = AppConstants.OcrEndpoint
                };

                var results = await computerVision.BatchReadFileInStreamAsync(photo.GetStream());
                var loc = results.OperationLocation;

                var operationId = loc.Substring(loc.Length - 36);


                var result = await computerVision.GetReadOperationResultAsync(operationId);

                // Wait for the operation to complete
                var i = 0;
                var maxRetries = 10;
                while ((result.Status == TextOperationStatusCodes.Running ||
                        result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
                {
                    await Task.Delay(1000);
                    result = await computerVision.GetReadOperationResultAsync(operationId);
                }

                if (result.Status == TextOperationStatusCodes.Running ||
                    result.Status == TextOperationStatusCodes.NotStarted)
                    return new List<string>();

                var all = result.RecognitionResults.SelectMany(l => l.Lines);

                return all.Select(l => l.Text).ToList();
            }

            return new List<string>();
        }

        async Task<string> GetValue(Func<string, bool> titleFunc)
        {
            var words = await GetLines();

            var titlePos = words.FindIndex(l => titleFunc(l));

            if (titlePos == -1)
            {
                return "";
            }

            var keyPos = titlePos + 1;
            return words[keyPos];
        }

        async Task Ok()
        {
            IsEnabled = false;
            try
            {
                var client = new CustomVisionPredictionClient
                {
                    ApiKey = PredictionKey,
                    Endpoint = Endpoint
                };

                if (Guid.TryParse(ProjectId, out var pId))
                {
                    var imagePath = "ObjectDetector.Images.single.png";
                    var assembly = typeof(SettingsViewModel).GetTypeInfo().Assembly;

                    using (var stream = assembly.GetManifestResourceStream(imagePath))
                    {
                        await client.DetectImageWithNoStoreAsync(pId, publishName, stream);
                    }

                    await KeyService.SetPredictionKey(PredictionKey);
                    await KeyService.SetProjectId(ProjectId);
                    await KeyService.SetPublishName(PublishName);

                    Analytics.TrackEvent("Updating keys");

                    await Application.Current.MainPage.Navigation.PopModalAsync();
                }
                else
                {
                    Analytics.TrackEvent("Failed updating keys", new Dictionary<string, string> { { "Error", "The project Id is not a valid GUID" } });
                    await Application.Current.MainPage.DisplayAlert("Error", "The project Id is not a valid GUID", "OK");
                }
            }
            catch (Exception ex)
            {
                Analytics.TrackEvent("Failed updating keys", new Dictionary<string, string> { { "Error", "The project Id and prediction key don't match an existing project" } });
                Crashes.TrackError(ex, new Dictionary<string, string> { { "Action", "Testing key and project id" } });
                await Application.Current.MainPage.DisplayAlert("Error", "The project Id and prediction key don't match an existing project", "OK");
            }
            finally
            {
                IsEnabled = true;
            }
        }

        public ICommand OpenCustomVisionCommand { get; }
        public ICommand ConfigureCanCancelCommand { get; }
        public Command OkCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand CapturePredictionKeyCommand { get; }
        public ICommand CaptureProjectIdCommand { get; }
        public ICommand CapturePublishNameCommand { get; }
        public ICommand CaptureEndpointCommand { get; }

        string predictionKey;
        public string PredictionKey
        {
            get => predictionKey;
            set
            {
                Set(ref predictionKey, value);
                OkCommand.ChangeCanExecute();
            }
        }

        string projectId;
        public string ProjectId
        {
            get => projectId;
            set
            {
                Set(ref projectId, value);
                OkCommand.ChangeCanExecute();
            }
        }

        string publishName;
        public string PublishName
        {
            get => publishName;
            set
            {
                Set(ref publishName, value);
                OkCommand.ChangeCanExecute();
            }
        }

        string endpoint;
        public string Endpoint
        {
            get => endpoint;
            set
            {
                Set(ref endpoint, value);
                OkCommand.ChangeCanExecute();
            }
        }

        bool canCancel;
        public bool CanCancel
        {
            get => canCancel;
            set => Set(ref canCancel, value);
        }
    }
}

