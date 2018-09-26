using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Humanizer;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.TextToSpeech;
using SkiaSharp;
using Xamarin.Forms;

namespace ObjectDetector
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
            TakePhotoCommand = new Command(async () => await TakePhoto());
            PickPhotoCommand = new Command(async () => await PickPhoto());
            ShowSettingsCommand = new Command(async () => await ShowSettings());
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        }

        async Task ShowSettings()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(SettingsPage.CreateSettingsPage(), true);
        }

        async Task PredictPhoto(MediaFile photo)
        {
            var endpoint = new PredictionEndpoint
            {
                ApiKey = await KeyService.GetPredictionKey()
            };

            var results = await endpoint.PredictImageAsync(Guid.Parse(await KeyService.GetProjectId()),
                                                           photo.GetStream());
            AllPredictions = results.Predictions
                                    .Where(p => p.Probability > Probability)
                                    .ToList();
        }

        SKBitmap image;
        public SKBitmap Image
        {
            get => image;
            set => Set(ref image, value);
        }

        double probability = .75;
        public double Probability
        {
            get => probability;
            set
            {
                if (Set(ref probability, value))
                {
                    OnPropertyChanged(nameof(ProbabilityText));
                    OnPropertyChanged(nameof(Predictions));
                }
            }
        }

        public string ProbabilityText => $"{Probability:P0}";

        List<PredictionModel> allPredictions = new List<PredictionModel>();
        public List<PredictionModel> AllPredictions
        {
            get => allPredictions;
            set
            {
                Set(ref allPredictions, value);
                OnPropertyChanged(nameof(Predictions));
            }
        }

        public List<PredictionModel> Predictions => AllPredictions.Where(p => p.Probability > Probability).ToList();

        public ICommand TakePhotoCommand { get; }
        public ICommand PickPhotoCommand { get; }
        public ICommand ShowSettingsCommand { get; }

        Task TakePhoto()
        {
            Analytics.TrackEvent("Take photo");
            return GetPhoto(() => CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions { PhotoSize = PhotoSize.Small }));
        }

        Task PickPhoto()
        {
            throw new NotImplementedException();
        }

        async Task GetPhoto(Func<Task<MediaFile>> getPhotoFunc)
        {
            IsEnabled = false;

            try
            {
                var photo = await getPhotoFunc();
                //if (photo == null) return;

                Image = null;
                AllPredictions = new List<PredictionModel>();

                Image = SKBitmap.Decode(photo.GetStreamWithImageRotatedForExternalStorage());
                await PredictPhoto(photo);

                IsEnabled = true;
                await SayWhatYouSee();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string> { { "Action", "Getting predictions" } });
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occured: {ex.Message}", "OK");
            }
            finally
            {
                IsEnabled = true;
            }
        }

        async Task SayWhatYouSee()
        {
            var text = "";

            try
            {
                if (Predictions.Any())
                {
                    if (Predictions.Count == 1)
                        text = $"I see {Predictions[0].TagName.Humanize()}";
                    else
                    {
                        text = "I see ";
                        for (var i = 0; i < Predictions.Count - 1; ++i)
                            text += Predictions[i].TagName.Humanize() + ", ";
                        text += $"and {Predictions.Last().TagName.Humanize()}";

                    }
                }
                else
                {
                    text = "I don't see anything I recognise";
                }

                await CrossTextToSpeech.Current.Speak(text);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string> { { "Action", "Speaking predictions" } });
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occured: {ex.Message}", "OK");
            }
        }
    }
}
