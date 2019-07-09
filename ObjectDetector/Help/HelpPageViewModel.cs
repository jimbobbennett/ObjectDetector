using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;

namespace ObjectDetector.Help
{
    public class HelpPageViewModel : BaseViewModel
    {
        int currentPage = 1;

        public HelpPageViewModel(INavigation navigation)
        {
            CloseCommand = new Command(async () =>
            {
                Analytics.TrackEvent("Close Help Early", new Dictionary<string, string> { { "Page", currentPage.ToString() } });
                await Application.Current.MainPage.Navigation.PopModalAsync();
            });

            ShowPage2Command = new Command(async () =>
            {
                Analytics.TrackEvent("Navigate to Help page 2");
                currentPage = 2;
                await navigation.PushAsync(new HelpPage2());
            });

            ShowPage3Command = new Command(async () =>
            {
                Analytics.TrackEvent("Navigate to Help page 3");
                Analytics.TrackEvent("Close Help");
                currentPage = 3;
                await navigation.PushAsync(new HelpPage3());
            });

            DoneCommand = new Command(async () =>
            {
                Analytics.TrackEvent("Close Help");
                await Application.Current.MainPage.Navigation.PopModalAsync();
            });
        }

        public ICommand CloseCommand { get; }

        public ICommand ShowPage2Command { get; }
        public ICommand ShowPage3Command { get; }
        public ICommand DoneCommand { get; }
    }
}
