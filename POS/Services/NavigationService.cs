using System;
using System.Linq;
using System.Windows;
using POS.Factories;
using POS.Services.Login;
using POS.Views.Windows;

namespace POS.Services
{
    public class NavigationService
    {
        private readonly IViewFactory _viewFactory;
        public NavigationService(WindowFactory windowFactory)
        {
            _viewFactory = windowFactory;
        }

        public void OpenLoginPanelWindow()
        {
            var loginView = new LoginPanelWindow();
            loginView.ShowDialog();
        }

        public void OpenNewWindowAndCloseCurrent<TNewWindowType>(TNewWindowType newWindowType, Action closeCurrentWindowAction) where TNewWindowType : Window
        {

            OpenNewWindow(typeof(TNewWindowType));

            if (Application.Current.Windows.OfType<TNewWindowType>().Any())
                closeCurrentWindowAction.Invoke();
        }

        public void OpenNewWindow(Type viewType)
        {
            if (viewType != typeof(MainWindow))
                HandleAuthenticatedNavigation(viewType);
            else
                HandleMainWindowNavigation();
        }

        public void CloseCurrentWindow<TWindowToClose>() where TWindowToClose : Window
        {
            var window = Application.Current.Windows.OfType<TWindowToClose>().FirstOrDefault();
            window?.Close();
        }

        private void HandleMainWindowNavigation()
        {
            LoginManager.Instance.LogOut();

            MainWindow mainWindow = new();
            mainWindow.Show();
        }

        private void HandleAuthenticatedNavigation(Type viewType)
        {
            LoginManager.Instance.IsAuthenticationOnlyRequired = true;
            OpenLoginPanelWindow();

            if (!LoginManager.Instance.SuccessfullyLoggedIn || (LoginManager.Instance.Employee == null || !LoginManager.Instance.Employee.IsUserLoggedIn))
                return;

            LoginManager.Instance.IsAuthenticationOnlyRequired = false;
            LoginManager.Instance.SuccessfullyLoggedIn = false;

            GetView(viewType);
        }

        private void GetView(Type viewType)
        { 
            var view = _viewFactory.GetView(viewType) as Window;
            view!.Show();
        }
    }
}
