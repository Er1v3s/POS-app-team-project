using System.Windows.Input;
using DataAccess.Models;
using POS.Services;
using POS.Services.WarehouseFunctions;
using POS.Utilities;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.WarehouseFunctions
{
    public class StockManagementViewModel : ViewModelBase
    {
        private readonly IngredientService _ingredientService;
        private readonly DeliveryService _deliveryService;
        private readonly StockManagementService _stockManagementService;

        private Ingredient? selectedIngredient;

        public MyObservableCollection<Ingredient> IngredientObservableCollection => _ingredientService.IngredientCollection;

        public Ingredient? SelectedIngredient
        {
            get => selectedIngredient;
            set => SetField(ref selectedIngredient, value);
        }

        public ICommand AddIngredientToDeliveryCommand { get; }
        public ICommand EditIngredientCommand { get; }

        public StockManagementViewModel(IngredientService ingeIngredientService, StockManagementService stockManagementService, DeliveryService deliveryService)
        {
            _ingredientService = ingeIngredientService;
            _stockManagementService = stockManagementService;
            _deliveryService = deliveryService;

            AddIngredientToDeliveryCommand = new RelayCommand(AddIngredientToDelivery);
            EditIngredientCommand = new RelayCommand(EditIngredientQuantity);
        }

        private void AddIngredientToDelivery()
        {
            if (selectedIngredient == null)
                return;

            _deliveryService.AddIngredientToDeliveryCollection(selectedIngredient);
        }

        private void EditIngredientQuantity()
        {
            if (selectedIngredient == null)
                return;

            _stockManagementService.EditIngredientQuantity(selectedIngredient);
        }
    }
}
