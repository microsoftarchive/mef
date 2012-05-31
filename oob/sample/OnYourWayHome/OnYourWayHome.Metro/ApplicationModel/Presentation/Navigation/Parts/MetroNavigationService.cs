using System;
using System.Composition;
using OnYourWayHome.ApplicationModel.Composition;
using OnYourWayHome.ViewModels.Parts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace OnYourWayHome.ApplicationModel.Presentation.Navigation.Parts
{
    // Metro's implementation of a navigation service that 
    // navigates from a ViewModel type to a View type
    internal class MetroNavigationService : NavigationService<Type>
    {
        public MetroNavigationService(CompositionContext compositionContext)
            : base(compositionContext)
        {
            Frame.Navigated += OnFrameNavigated;

            Register(typeof(ShoppingListViewModel),     typeof(ShoppingListView));
            Register(typeof(AddGroceryItemViewModel),   typeof(AddGroceryItemView));
        }

        public Frame Frame
        {
            get { return (Frame)Window.Current.Content; }
        }

        public override bool CanGoBack
        {
            get { return Frame.CanGoBack; }
        }

        protected override void NavigateTo(Type viewType)
        {
            Frame.Navigate(viewType);
        }

        protected override void GoBackCore()
        {
            Frame.GoBack();
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                Bind(e.SourcePageType, (MetroView)e.Content);
            }
        }
    }
}
