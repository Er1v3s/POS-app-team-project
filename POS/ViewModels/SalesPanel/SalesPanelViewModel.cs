using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using POS.Services;
using POS.Services.Login;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.SalesPanel
{
    public class SalesPanelViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;

        private string loggedInUserName;

        public string LoggedInUserName
        {
            get => loggedInUserName;
            set => SetField(ref loggedInUserName, value);
        }

        public Action CloseWindowAction;
        public ICommand MoveToMainWindowCommand { get; }

        public SalesPanelViewModel(NavigationService navigationService)
        {
            _navigationService = navigationService;

            MoveToMainWindowCommand = new RelayCommand(MoveToMainWindow);

            loggedInUserName = LoginManager.Instance.GetLoggedInUserFullName();
        }

        private void MoveToMainWindow(object obj)
        {
            _navigationService.OpenMainWindow();

            if(Application.Current.Windows.OfType<Views.Windows.MainWindow>().Any())
                CloseWindowAction.Invoke();
        }
    }
}
