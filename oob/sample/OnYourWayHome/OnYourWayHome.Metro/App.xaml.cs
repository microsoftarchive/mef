using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OnYourWayHome
{
    public sealed partial class OnYourWayHomeMetroHost : Application
    {
        private readonly OnYourWayHomeApplication _application = new OnYourWayHomeApplication(typeof(OnYourWayHomeMetroHost));

        public OnYourWayHomeMetroHost()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Window.Current.Content = new Frame();
            Window.Current.Activate();

            _application.Start();
        }
    }
}
