﻿using System.Linq;
using System.Windows;
using POS.Services.Login;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;
using System.Windows.Input;
using POS.Services;
using DataAccess.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using POS.Models.Warehouse;
using POS.Services.WarehouseFunctions;
using POS.Utilities;

namespace POS.ViewModels.WarehouseFunctions
{
    public class CreateDeliveryViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private readonly IngredientService _ingredientService;
        private readonly DeliveryService _deliveryService;

        private const string DefaultPlaceholder = "Wpisz nazwę...";
        private string searchPhrase = DefaultPlaceholder;
        private string placeholder = DefaultPlaceholder;

        private Ingredient? selectedIngredient;

        public MyObservableCollection<Ingredient> IngredientObservableCollection
        {
            get => _ingredientService.IngredientCollection;
        }
        public ObservableCollection<DeliveryDto> DeliveryObservableCollection => _deliveryService.DeliveryCollection;

        public Ingredient? SelectedIngredient
        {
            get => selectedIngredient;
            set => SetField(ref selectedIngredient, value);
        }

        public static string LoggedInUserName => LoginManager.Instance.GetLoggedInUserFullName();

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

        public CreateDeliveryViewModel(
            NavigationService navigationService,
            IngredientService ingredientService,
            DeliveryService deliveryService)
        {
            _navigationService = navigationService;
            _ingredientService = ingredientService;
            _deliveryService = deliveryService;

            AddIngredientToDeliveryCommand = new RelayCommand(AddIngredientToDelivery);
            IncreaseIngredientQuantityCommand = new RelayCommand<DeliveryDto>(IncreaseIngredientQuantity);
            DeleteIngredientFromDeliveryCommand = new RelayCommand<Ingredient>(DeleteIngredientFromDelivery);
            EditIngredientCommand = new RelayCommand(EditIngredientQuantity);
            CancelDeliveryCommand = new RelayCommand(CancelDelivery);
            OpenMainWindowCommand = new RelayCommand<Views.Windows.MainWindow>(OpenMainWindow);
        }

        private void ShowAllIngredients()
        {
            Task.Run(_ingredientService.GetAllIngredients);
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

        private void AddIngredientToDelivery()
        {
            if (selectedIngredient == null) 
                return;
            
            _deliveryService.AddIngredientToDeliveryCollection(selectedIngredient);
        }

        private void IncreaseIngredientQuantity(DeliveryDto deliveryDto)
        {
            _deliveryService.IncreaseIngredientQuantity(deliveryDto);
        }

        private void DeleteIngredientFromDelivery(Ingredient ingredient)
        {
            _deliveryService.DeleteIngredientFromDeliveryCollection(ingredient);
        }

        private void EditIngredientQuantity()
        {
            if(selectedIngredient == null)
                return;

            _deliveryService.EditIngredientQuantity(selectedIngredient);
        }

        private void CancelDelivery()
        {
            var result = MessageBox.Show("Czy na pewno chcesz anulować zamówienie?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                _deliveryService.CancelDelivery();
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
