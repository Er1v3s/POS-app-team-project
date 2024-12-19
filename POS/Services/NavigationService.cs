using System;
using POS.Factories;
using POS.Services.Login;
using POS.Views.RegisterSale;

namespace POS.Services
{
    public class NavigationService
    {
        private readonly ViewFactory _viewFactory;
        private object view;
        
        public NavigationService(ViewFactory viewFactory)
        {
            _viewFactory = viewFactory;
        }

        public void OpenLoginPanelWindow()
        {
            LoginManager.OpenLoginWindow();
        }

        public void OpenSalesPanelWindow()
        {
            LoginManager.Instance.IsAuthenticationOnlyRequired = true;
            LoginManager.OpenLoginWindow();

            if (LoginManager.Instance.SuccessfullyLoggedIn && LoginManager.Instance.Employee!.IsUserLoggedIn)
            {
                LoginManager.Instance.IsAuthenticationOnlyRequired = false;
                LoginManager.Instance.SuccessfullyLoggedIn = false;

                var salesPanel = new SalesPanel(LoginManager.Instance.Employee.EmployeeId);
                salesPanel.Show();
            }
        }

        public object GetViewSource(object commandParameter)
        {
            var convertedParameter = Convert.ToInt32(commandParameter);

            if (convertedParameter == 0)
                 GetView(convertedParameter);
            else
                CheckIfUserIsLoggedInAndGetView(convertedParameter);

            return view;
        }

        private void GetView(int parameter)
        {
            view = _viewFactory.GetView(parameter);
        }

        private void CheckIfUserIsLoggedInAndGetView(int parameter)
        {
            if (LoginManager.Instance.IsAnySessionActive)
                GetView(parameter);
            else
                LoginManager.OpenLoginWindow();
        }
    }
}
