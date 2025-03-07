using POS.Services.Login;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;
using System.Windows.Input;
using System.Windows;
using POS.Services;
using POS.Views.Windows.WarehouseFunctions;

namespace POS.ViewModels.WarehouseFunctions
{
    public class ProductManagementViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;

        private string loggedInUserName;

        public string LoggedInUserName
        {
            get => loggedInUserName;
            set => SetField(ref loggedInUserName, value);
        }

        public ICommand OpenMainWindowCommand { get; }

        public ProductManagementViewModel(NavigationService navigationService)
        {
            _navigationService = navigationService;

            OpenMainWindowCommand = new RelayCommand<Views.Windows.MainWindow>(OpenMainWindow);

            loggedInUserName = LoginManager.Instance.GetLoggedInUserFullName();
        }

        private void OpenMainWindow<T>(T windowType) where T : Window
        {
            _navigationService.OpenNewWindowAndCloseCurrent(windowType, () => _navigationService.CloseCurrentWindow<ProductManagementWindow>());
        }
    }
}
