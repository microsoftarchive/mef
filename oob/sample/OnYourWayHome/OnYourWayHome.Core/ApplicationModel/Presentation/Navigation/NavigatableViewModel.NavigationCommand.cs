using System;
using OnYourWayHome.ApplicationModel.Eventing;
using OnYourWayHome.ApplicationModel.Presentation.Input;

namespace OnYourWayHome.ApplicationModel.Presentation.Navigation
{
    partial class NavigatableViewModel
    {
        private class NavigationCommand : ActionCommand
        {
            private readonly INavigationService _navigationService;

            public NavigationCommand(Action action, INavigationService navigationService)
                : base(action)
            {
                Assumes.NotNull(navigationService);

                _navigationService = navigationService;
                _navigationService.CanGoBackChanged += EventServices.MakeWeak(OnCanGoBackChanged, h => _navigationService.CanGoBackChanged -= h);
            }

            public override bool CanExecute()
            {
                return _navigationService.CanGoBack;
            }

            public void OnCanGoBackChanged(object sender, EventArgs e)
            {
                FireCanExecuteChanged();
            }
        }
    }
}
