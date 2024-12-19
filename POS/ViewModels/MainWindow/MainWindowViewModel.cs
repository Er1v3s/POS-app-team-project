using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using POS.Services.Login;
using POS.Utilities;
using POS.Utilities.RelayCommands;
using POS.Views.RegisterSale;

namespace POS.ViewModels.MainWindow
{
    public class MainWindowViewModel : ViewModelBase
    {

        public Action TurnOffApplicationAction;
        public Action CloseWindowsAction;
        public ICommand NavigateToSalesPanelCommand { get; }
        public ICommand TurnOffApplicationCommand { get; }

        public MainWindowViewModel()
        {
            NavigateToSalesPanelCommand = new RelayCommand(NavigateToSalesPanel);
            TurnOffApplicationCommand = new RelayCommand(TurnOffApplication);
        }

        public void NavigateToSalesPanel(object obj)
        {
            LoginManager.Instance.IsAuthenticationOnlyRequired = true;
            LoginManager.OpenLoginWindow();

            if (LoginManager.Instance.SuccessfullyLoggedIn && LoginManager.Instance.Employee!.IsUserLoggedIn)
            {
                LoginManager.Instance.IsAuthenticationOnlyRequired = false;
                LoginManager.Instance.SuccessfullyLoggedIn = false;

                SalesPanel salesPanel = new SalesPanel(LoginManager.Instance.Employee.EmployeeId);
                salesPanel.Show();

                CloseWindowsAction.Invoke();
            }
        }

        public void ChangeSource(object obj)
        {

        }

        private void TurnOffApplication(object obj)
        {
            TurnOffApplicationAction.Invoke();
        }
    }
}
