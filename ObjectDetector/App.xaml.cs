using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ObjectDetector
{
    public partial class App : Xamarin.Forms.Application
    {
        public App()
        {
            InitializeComponent();

            var navPage = new Xamarin.Forms.NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.DarkSlateBlue,
                BarTextColor = Color.White
            };
            navPage.On<iOS>().SetPrefersLargeTitles(true);

            MainPage = navPage;

            AppCenter.Start(AppConstants.AppCenterKey,
                            typeof(Analytics),
                            typeof(Crashes),
                            typeof(Distribute),
                            typeof(Push));

            Push.PushNotificationReceived += OnPushRecieved;
        }

        void OnPushRecieved(object sender, PushNotificationReceivedEventArgs e)
        {
            if (e.CustomData.TryGetValue("CheckIsInstalled", out var s))
            {
                Analytics.TrackEvent("App is still installed");
            }
        }

        protected override async void OnStart()
        {
            if (string.IsNullOrWhiteSpace(await KeyService.GetPredictionKey()) ||
                string.IsNullOrWhiteSpace(await KeyService.GetProjectId()))
            {
                await MainPage.Navigation.PushModalAsync(SettingsPage.CreateSettingsPage(), false);
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
