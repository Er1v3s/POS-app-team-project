using System;
using System.Linq;
using System.Windows;
using POS.Factories;
using POS.Services.Login;
using POS.Views.Windows;
using POS.Views.Windows.SalesPanel;
using POS.Views.Windows.WarehouseFunctions;

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

        public void OpenNewWindow<T>(T windowType)
        {
            if (typeof(T) == typeof(MainWindow))
            {
                LoginManager.Instance.LogOut();

                MainWindow mainWindow = new ();
                mainWindow.Show();
            }
            else
            {
                LoginManager.Instance.IsAuthenticationOnlyRequired = true;
                LoginManager.OpenLoginWindow();

                if (LoginManager.Instance.SuccessfullyLoggedIn && LoginManager.Instance.Employee!.IsUserLoggedIn)
                {
                    LoginManager.Instance.IsAuthenticationOnlyRequired = false;
                    LoginManager.Instance.SuccessfullyLoggedIn = false;

                    if (typeof(T) == typeof(SalesPanelWindow))
                    {
                        SalesPanelWindow salesPanelWindow = new();
                        salesPanelWindow.Show();
                    }
                    else if (typeof(T) == typeof(StockManagementWindow))
                    {
                        StockManagementWindow stockManagementWindow = new();
                        stockManagementWindow.Show();
                    }
                    else if (typeof(T) == typeof(CreateDeliveryWindow))
                    {
                        CreateDeliveryWindow createDeliveryWindow = new();
                        createDeliveryWindow.Show();
                    }
                }
            }
        }

        public void CloseCurrentWindow<T>() where T : Window
        {
            var window = Application.Current.Windows.OfType<T>().FirstOrDefault();
            window?.Close();
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
