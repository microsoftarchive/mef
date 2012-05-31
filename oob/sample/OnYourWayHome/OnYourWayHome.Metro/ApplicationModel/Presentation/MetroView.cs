using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace OnYourWayHome.ApplicationModel.Presentation
{
    // Metro's implementation of a View
    public class MetroView : Page, IView
    {
        public MetroView()
        {
        }

        public void Bind(object context)
        {
            Requires.NotNull(context, "context");

            DataContext = context;
        }
    }
}
