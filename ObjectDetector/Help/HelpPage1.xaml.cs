using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ObjectDetector.Help
{
    public partial class HelpPage1 : ContentPage
    {
        public HelpPage1()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
