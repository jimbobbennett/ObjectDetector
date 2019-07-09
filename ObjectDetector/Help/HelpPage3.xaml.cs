using System;
using System.Collections.Generic;

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
    }
}
