using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using POS.Services.Login;
using POS.ViewModels.StartFinishWork;
using POS.Views.Base;
using POS.Views.UserControls.LoginPanelWindow;

namespace POS.Views.Windows
{
    public partial class LoginPanelWindow : WindowBase
    {
        public LoginPanelWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<LoginPanelViewModel>();
            
            var viewModel = (LoginPanelViewModel)DataContext;
            viewModel.CloseWindowBaseAction = Close;
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
                    var startFinishWork = new StartFinishWorkUserControl();
                    LoginPanel.Child = startFinishWork;

                    startFinishWork.StartWork.Click += CloseWindow_ButtonClick;
                    startFinishWork.FinishWork.Click += CloseWindow_ButtonClick;
                }
            }
        }

        private void CloseWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}