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

        private string amountOfIngredientInRecipe;
        private string newProductName;
        private string productCategory;
        private string productPrice;
        private string productDescription;
        private string productRecipe;
        private string newIngredientName;
        private string ingredientUnit;
        private string ingredientPackage;
        private string ingredientDescription;

        public string LoggedInUserName
        {
            get => loggedInUserName;
            set => SetField(ref loggedInUserName, value);
        }

        public string AmountOfIngredientInRecipe
        {
            get => amountOfIngredientInRecipe;
            set => SetField(ref amountOfIngredientInRecipe, value);
        }

        public string ProductCategory
        {
            get => productCategory;
            set => SetField(ref productCategory, value);
        }

        public string ProductPrice
        {
            get => productPrice;
            set => SetField(ref productPrice, value);
        }
        
        public string NewProductName
        {
            get => newProductName;
            set => SetField(ref newProductName, value);
        }
        
        public string ProductDescription
        {
            get => productDescription;
            set => SetField(ref productDescription, value);
        }
        
        public string ProductRecipe
        {
            get => productRecipe;
            set => SetField(ref productRecipe, value);
        }
        
        public string NewIngredientName
        {
            get => newIngredientName;
            set => SetField(ref newIngredientName, value);
        }
        
        public string IngredientUnit
        {
            get => ingredientUnit;
            set => SetField(ref ingredientUnit, value);
        }
        
        public string IngredientPackage
        {
            get => ingredientPackage;
            set => SetField(ref ingredientPackage, value);
        }
        
        public string IngredientDescription
        {
            get => ingredientDescription;
            set => SetField(ref ingredientDescription, value);
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
