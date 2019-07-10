using System.Collections.Generic;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;

namespace ObjectDetector.Help
{
    public class HelpPage : NavigationPage
    {
        public HelpPage() : base(new HelpPage1())
        {
            SetHasBackButton(this, false);
            SetHasNavigationBar(this, false);

            BindingContext = new HelpPageViewModel(Navigation);

            PushAsync(new HelpPage1(), false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Analytics.TrackEvent("Loaded View", new Dictionary<string, string> { { "View", nameof(HelpPage) } });
        }
    }
}
