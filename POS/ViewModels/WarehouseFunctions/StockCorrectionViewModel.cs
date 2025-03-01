using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using POS.Services;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.WarehouseFunctions
{
    public class StockCorrectionViewModel : ViewModelBase
    {
        private readonly IngredientService _ingredientService;

        private Ingredient ingredient;
        private int ingredientSafetyStock;
        private int ingredientStock;

        private bool dialogResult;

        public Ingredient Ingredient
        {
            get => ingredient;
            set => SetField(ref ingredient, value);
        }

        public int IngredientSafetyStock
        {
            get => ingredientSafetyStock;
            set => SetField(ref ingredientSafetyStock, value);
        }

        public int IngredientStock
        {
            get => ingredientStock;
            set => SetField(ref ingredientStock, value);
        }

        public bool DialogResult
        {
            get => dialogResult;
            set => SetField(ref dialogResult, value);
        }

        public ICommand SetSelectedIngredientCommand { get; }
        public ICommand LoadSelectedIngredientDataCommand { get; }
        public ICommand IncreaseStockValueCommand { get; }
        public ICommand DecreaseStockValueCommand { get; }
        public ICommand IncreaseSafetyStockValueCommand { get; }
        public ICommand DecreaseSafetyStockValueCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand CancelChangesCommand { get; }

        public StockCorrectionViewModel(IngredientService ingredientService)
        {
            _ingredientService = ingredientService;

            SetSelectedIngredientCommand = new RelayCommand<Ingredient>(SetSelectedIngredient);
            LoadSelectedIngredientDataCommand = new RelayCommand(LoadSelectedIngredientData);
            IncreaseStockValueCommand = new RelayCommand(IncreaseStockValue);
            DecreaseStockValueCommand = new RelayCommand(DecreaseStockValue);
            IncreaseSafetyStockValueCommand = new RelayCommand(IncreaseSafetyStockValue);
            DecreaseSafetyStockValueCommand = new RelayCommand(DecreaseSafetyStockValue);
            SaveChangesCommand = new RelayCommandAsync(SaveChangesAsync);
            CancelChangesCommand = new RelayCommand(CancelChanges);
        }

        private void SetSelectedIngredient(Ingredient ingredientArg)
        {
            ingredient = ingredientArg;
        }

        private void LoadSelectedIngredientData()
        {
            ingredientSafetyStock = ingredient.SafetyStock;
            ingredientStock = ingredient.Stock;
        }

        private void IncreaseStockValue()
        {
            IngredientStock++;
        }

        private void DecreaseStockValue()
        {
            IngredientStock--;
        }

        private void IncreaseSafetyStockValue()
        {
            IngredientSafetyStock++;
        }

        private void DecreaseSafetyStockValue()
        {
            IngredientSafetyStock--;
        }

        private async Task SaveChangesAsync()
        {
            try
            {
                var updatedIngredient = CreateIngredientDto();
                await _ingredientService.UpdateIngredientQuantityAsync(updatedIngredient);

                DialogResult = true;
                CloseWindowBaseAction!.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się zapisać zmian, przyczyna: {ex.Message}");
            }
        }

        private Ingredient CreateIngredientDto()
        {
            return new Ingredient
            {
                IngredientId = ingredient.IngredientId,
                Name = ingredient.Name,
                Description = ingredient.Description,
                Unit = ingredient.Unit,
                Package = ingredient.Package,
                Stock = ingredientStock,
                SafetyStock = ingredientSafetyStock,
            };
        }

        private void CancelChanges()
        {
            DialogResult = false;
            CloseWindowBaseCommand.Execute(null);
        }
    }
}
