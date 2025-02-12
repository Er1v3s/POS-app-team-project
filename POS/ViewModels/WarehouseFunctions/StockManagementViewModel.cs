using System.Linq;
using System.Windows;
using System.Windows.Input;
using POS.Services;
using POS.Services.Login;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.WarehouseFunctions
{
    public class StockManagementViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;

        private string loggedInUserName;

        public string LoggedInUserName
        {
            get => loggedInUserName;
            set => SetField(ref loggedInUserName, value);
        }

        public ICommand OpenMainWindowCommand { get; }

        public StockManagementViewModel(NavigationService navigationService)
        {
            _navigationService = navigationService;

            OpenMainWindowCommand = new RelayCommand<Views.Windows.MainWindow>(OpenMainWindow);

            loggedInUserName = LoginManager.Instance.GetLoggedInUserFullName();
        }

        private void OpenMainWindow<T>(T windowType)
        {
            _navigationService.OpenWindow(windowType);

            if (Application.Current.Windows.OfType<T>().Any())
                CloseWindowBaseAction!.Invoke();
        }
    }
}
