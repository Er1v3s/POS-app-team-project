using System.Linq;
using System.Windows;
using POS.Services.Login;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;
using System.Windows.Input;
using POS.Services;
using DataAccess.Models;
using System.Collections.ObjectModel;
using Microsoft.IdentityModel.Tokens;
using POS.Models.Warehouse;
using POS.Utilities;
using POS.Views.Windows.WarehouseFunctions;

namespace POS.ViewModels.WarehouseFunctions
{
    public class CreateDeliveryViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private readonly IngredientService _ingredientService;

        private MyObservableCollection<Ingredient> ingredientCollection;
        private ObservableCollection<DeliveryDto> deliveryCollection;

        private Ingredient selectedIngredient;

        private string loggedInUserName;

        private const string DefaultPlaceholder = "Wpisz nazwę...";

        private string searchPhrase = DefaultPlaceholder;
        private string placeholder = DefaultPlaceholder;


        public MyObservableCollection<Ingredient> IngredientObservableCollection
        {
            get => ingredientCollection;
            set => SetField(ref ingredientCollection, value);
        }

        public ObservableCollection<DeliveryDto> DeliveryObservableCollection
        {
            get => deliveryCollection;
            set => SetField(ref deliveryCollection, value);
        }

        public Ingredient SelectedIngredient
        {
            get => selectedIngredient;
            set => SetField(ref selectedIngredient, value);
        }

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

        public ICommand AddIngredientToDeliveryCommand { get; }
        public ICommand IncreaseIngredientQuantityCommand { get; }
        public ICommand DeleteIngredientFromDeliveryCommand { get; }
        public ICommand EditIngredientCommand { get; }
        public ICommand CancelDeliveryCommand { get; }
        public ICommand OpenMainWindowCommand { get; }

        public CreateDeliveryViewModel(NavigationService navigationService, IngredientService ingredientService)
        {
            _navigationService = navigationService;
            _ingredientService = ingredientService;

            AddIngredientToDeliveryCommand = new RelayCommand(AddIngredientToDelivery);
            IncreaseIngredientQuantityCommand = new RelayCommand<DeliveryDto>(IncreaseIngredientQuantity);
            DeleteIngredientFromDeliveryCommand = new RelayCommand<Ingredient>(DeleteIngredientFromDelivery);
            EditIngredientCommand = new RelayCommand(EditIngredientQuantity);
            CancelDeliveryCommand = new RelayCommand(CancelDelivery);
            OpenMainWindowCommand = new RelayCommand<Views.Windows.MainWindow>(OpenMainWindow);

            loggedInUserName = LoginManager.Instance.GetLoggedInUserFullName();

            IngredientObservableCollection = new();
            deliveryCollection = new();
            ShowAllIngredients();
        }

        private void ShowAllIngredients()
        {
            IngredientObservableCollection.Clear();

            var ingredients = _ingredientService.GetAllIngredients();
            IngredientObservableCollection.AddRange(ingredients);
        }

        private void FilterIngredientsBySearchPhrase(string searchPhraseArg)
        {
            var filteredIngredients = _ingredientService.GetIngredientsBySearchPhrase(searchPhraseArg);
            IngredientObservableCollection.AddRange(filteredIngredients);
        }

        private void HandleProductFiltering(string searchPhraseArg)
        {
            if (searchPhraseArg.IsNullOrEmpty() || searchPhraseArg == DefaultPlaceholder)
                ShowAllIngredients();
            else
                FilterIngredientsBySearchPhrase(searchPhraseArg);
        }

        private void AddIngredientToDelivery()
        {
            var existingIngredient = deliveryCollection.FirstOrDefault(i => i.Ingredient.IngredientId == selectedIngredient.IngredientId);

            if (existingIngredient == null)
            {
                var deliveryItem = new DeliveryDto
                {
                    Ingredient = selectedIngredient,
                    Quantity = 1
                };

                DeliveryObservableCollection.Add(deliveryItem);
            }
            else
                existingIngredient.Quantity++;

        }

        private void IncreaseIngredientQuantity(DeliveryDto deliveryDto)
        {
            deliveryDto.Quantity++;
        }

        private void DeleteIngredientFromDelivery(Ingredient ingredient)
        {
            var existingIngredient = deliveryCollection.FirstOrDefault(i => i.Ingredient.IngredientId == ingredient.IngredientId);

            if (existingIngredient!.Quantity == 1)
            {
                var result = MessageBox.Show("Czy usunąć składnik z listy całkowicie?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    DeliveryObservableCollection.Remove(existingIngredient);
            }
            else
                existingIngredient.Quantity--;
        }

        private void EditIngredientQuantity()
        {
            var stockCorrection = new StockCorrectionWindow(selectedIngredient);
            var dialogResult = stockCorrection.ShowDialog();

            if (dialogResult == true)
                ShowAllIngredients();
        }

        private void CancelDelivery()
        {
            var result = MessageBox.Show("Czy na pewno chcesz anulować zamówienie?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
                DeliveryObservableCollection.Clear();
        }

        private void HandlePlaceholder()
        {
            Placeholder = SearchPhrase.IsNullOrEmpty() || SearchPhrase == DefaultPlaceholder
                ? DefaultPlaceholder
                : string.Empty;
        }

        private void OpenMainWindow<T>(T windowType)
        {
            _navigationService.OpenWindow(windowType);

            if (Application.Current.Windows.OfType<T>().Any())
                CloseWindowBaseAction!.Invoke();
        }
    }
}
