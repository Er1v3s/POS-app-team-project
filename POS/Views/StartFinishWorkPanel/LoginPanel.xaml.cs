using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using POS.Services.Login;
using POS.ViewModels.StartFinishWork;

namespace POS.Views.StartFinishWorkPanel
{
    public partial class LoginPanel : Window
    {
        private readonly string uri;
        public LoginPanel(string uri = "")
        {
            this.uri = uri;

            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<LoginPanelViewModel>();
        }

        private void LogIn_ButtonClick(object sender, RoutedEventArgs e)
        {
            LoginPanelViewModel loginPanelViewModel = (LoginPanelViewModel)DataContext;
            loginPanelViewModel.LoginCommand.Execute(null);

            if (LoginManager.Instance.Employee != null)
            {
                if (LoginManager.Instance.IsAuthenticationOnlyRequired && LoginManager.Instance.Employee.IsUserLoggedIn)
                {
                    Close();
                }
                else
                {
                    StartFinishWork startFinishWork = new StartFinishWork();
                    loginPanelWindow.Child = startFinishWork;

                    startFinishWork.StartWork.Click += CloseWindow_ButtonClick;
                    startFinishWork.FinishWork.Click += CloseWindow_ButtonClick;
                }
            }
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void CloseWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}