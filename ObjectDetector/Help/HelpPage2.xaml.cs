using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ObjectDetector.Help
{
    public partial class HelpPage2 : ContentPage
    {
        public HelpPage2()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
