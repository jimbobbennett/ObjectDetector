using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;

namespace ObjectDetector.Help
{
    public partial class HelpPage3 : ContentPage
    {
        public HelpPage3()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Analytics.TrackEvent("Loaded View", new Dictionary<string, string> { { "View", nameof(HelpPage3) } });
        }
    }
}
