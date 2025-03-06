using POS.Factories;
using POS.Services.Login;
using System;
using POS.Views.UserControls.MainWindow;
using POS.Views.Windows;

namespace POS.Services
{
    public class ViewService
    {
        private readonly IViewFactory _viewFactory;
        private readonly NavigationService _navigationService;
        private object view;

        public ViewService(UserControlFactory userControlFactory, NavigationService navigationService)
        {
            _viewFactory = userControlFactory;
            _navigationService = navigationService;
        }

        public object GetViewSource(object commandParameter)
        {
            var convertedParameter = Convert.ToInt32(commandParameter);

            switch (convertedParameter)
            {
                case 1:
                    CheckIfUserIsLoggedInAndGetView(typeof(WarehouseFunctionsUserControl));
                    break;
                case 2:
                    CheckIfUserIsLoggedInAndGetView(typeof(ReportsAndAnalysisUserControl));
                    break;
                case 3:
                    CheckIfUserIsLoggedInAndGetView(typeof(AdminFunctionsUserControl));
                    break;
                default:
                    GetView(typeof(WorkTimeSummaryUserControl));
                    break;
            }

            return view;
        }

        private void GetView(Type viewType)
        {
            view = _viewFactory.GetView(viewType);
        }

        private void CheckIfUserIsLoggedInAndGetView(Type viewType)
        {
            if (LoginManager.Instance.IsAnySessionActive)
                GetView(viewType);
            else
                _navigationService.OpenLoginPanelWindow();
        }
    }
}
