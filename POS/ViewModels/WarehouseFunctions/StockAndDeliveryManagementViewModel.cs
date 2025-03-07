using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.IdentityModel.Tokens;
using POS.Services;
using POS.Services.Login;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;
using POS.Views.Windows.WarehouseFunctions;

namespace POS.ViewModels.WarehouseFunctions
{
    public class StockAndDeliveryManagementViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private readonly IngredientService _ingredientService;

        private string loggedInUserName;

        private const string DefaultPlaceholder = "Wpisz nazwę...";
        private string searchPhrase = DefaultPlaceholder;
        private string placeholder = DefaultPlaceholder;

        public string LoggedInUserName
        {
            get => loggedInUserName;
            set => SetField(ref loggedInUserName, value);
        }

        public string SearchPhrase
        {
            get => searchPhrase;
            set
            {
                if (SetField(ref searchPhrase, value))
                {
                    HandlePlaceholder();
                    HandleProductFiltering(value);
                }
            }
        }

        public string Placeholder
        {
            get => placeholder;
            set => SetField(ref placeholder, value);
        }

        public ICommand OpenMainWindowCommand { get; }

        public StockAndDeliveryManagementViewModel(NavigationService navigationService, IngredientService ingredientService)
        {
            _ingredientService = ingredientService;
            _navigationService = navigationService;

            OpenMainWindowCommand = new RelayCommand<Views.Windows.MainWindow>(OpenMainWindow);

            loggedInUserName = LoginManager.Instance.GetLoggedInUserFullName();
        }

        private void ShowAllIngredients()
        {
            _ingredientService.GetAllIngredients();
        }

        private void FilterIngredientsBySearchPhrase(string searchPhraseArg)
        {
            _ingredientService.GetIngredientsBySearchPhrase(searchPhraseArg);
        }

        private void HandleProductFiltering(string searchPhraseArg)
        {
            if (searchPhraseArg.IsNullOrEmpty() || searchPhraseArg == DefaultPlaceholder)
                ShowAllIngredients();
            else
                FilterIngredientsBySearchPhrase(searchPhraseArg);
        }

        private void HandlePlaceholder()
        {
            Placeholder = SearchPhrase.IsNullOrEmpty() || SearchPhrase == DefaultPlaceholder
                ? DefaultPlaceholder
                : string.Empty;
        }

        private void OpenMainWindow<T>(T windowType) where T : Window
        {
            _navigationService.OpenNewWindowAndCloseCurrent(windowType, () => _navigationService.CloseCurrentWindow<StockAndDeliveryManagementWindow>());
        }
    }
}
