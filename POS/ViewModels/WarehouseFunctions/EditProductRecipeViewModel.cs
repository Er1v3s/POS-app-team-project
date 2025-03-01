using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Microsoft.IdentityModel.Tokens;
using POS.Services;
using POS.Services.SalesPanel;
using POS.Utilities;
using POS.Utilities.RelayCommands;
using POS.Validators.Models;
using POS.ViewModels.Base.WarehouseFunctions;

namespace POS.ViewModels.WarehouseFunctions
{
    public class EditProductRecipeViewModel : FormViewModelBase
    {
        private readonly ProductService _productService;
        private readonly IngredientService _ingredientService;
        private readonly RecipeService _recipeService;
        private readonly RecipeIngredientService _recipeIngredientService;

        private readonly RecipeIngredientValidator _recipeIngredientValidator;

        private Product? selectedProduct;
        private Ingredient? selectedIngredient;
        private RecipeIngredient? selectedRecipeIngredient;

        private string amountOfIngredient;
        private string amountOfIngredientError;

        private Visibility isProductSelected;
        private Visibility isIngredientSelected;

        public MyObservableCollection<Product> ProductObservableCollection => _productService.ProductCollection;
        public MyObservableCollection<Ingredient> IngredientObservableCollection => _ingredientService.IngredientCollection;
        public MyObservableCollection<RecipeIngredient> RecipeIngredientCollection => _recipeIngredientService.RecipeIngredientCollection;

        public Product? SelectedProduct
        {
            set
            {
                if (SetField(ref selectedProduct, value))
                {
                    IsProductSelected = Visibility.Collapsed;
                    _ = GetRecipeIngredientsAsync(value!);
                }
            }
        }

        public Ingredient? SelectedIngredient
        {
            set
            {
                if (SetField(ref selectedIngredient, value))
                {
                    IsIngredientSelected = Visibility.Collapsed;

                    if (value is not null)
                    {
                        IsAddButtonVisible = Visibility.Visible;
                        IsUpdateButtonVisible = Visibility.Collapsed;
                    }
                }
            }
        }

        public RecipeIngredient? SelectedRecipeIngredient
        {
            get => selectedRecipeIngredient;
            set
            {
                if (SetField(ref selectedRecipeIngredient, value))
                {
                    SelectedIngredient = null;
                    IsIngredientSelected = Visibility.Visible;

                    if (value is not null)
                    {
                        LoadDataIntoFormFields(value!);
                        IsUpdateButtonVisible = Visibility.Visible;
                        IsAddButtonVisible = Visibility.Collapsed;
                    }

                    IsDeleteButtonEnable = CheckIfDeleteButtonCanBeEnabled();
                }
            }
        }

        public string AmountOfIngredient
        {
            get => amountOfIngredient;
            set
            {
                if (SetField(ref amountOfIngredient, value))
                    ValidateProperty(_recipeIngredientValidator.ValidateQuantity, nameof(AmountOfIngredient), value, error => AmountOfIngredientError = error);
            }
        }
        
        public string AmountOfIngredientError
        {
            get => amountOfIngredientError;
            set => SetField(ref amountOfIngredientError, value);
        }

        public Visibility IsProductSelected
        {
            get => isProductSelected;
            set => SetField(ref isProductSelected, value);
        }

        public Visibility IsIngredientSelected
        {
            get => isIngredientSelected;
            set => SetField(ref isIngredientSelected, value);
        }

        public ICommand AddIngredientToRecipeCommand { get; }
        public ICommand UpdateIngredientInRecipeCommand { get; }
        public ICommand DeleteIngredientFromRecipeCommand { get; }

        public EditProductRecipeViewModel(
            ProductService productService,
            RecipeService recipeService,
            IngredientService ingredientService,
            RecipeIngredientService recipeIngredientService)
        {
            _productService = productService;
            _recipeService = recipeService;
            _ingredientService = ingredientService;
            _recipeIngredientService = recipeIngredientService;

            _recipeIngredientValidator = new RecipeIngredientValidator();

            AddIngredientToRecipeCommand = new RelayCommandAsync(AddIngredientToRecipe);
            UpdateIngredientInRecipeCommand = new RelayCommandAsync(UpdateIngredientInRecipe);
            DeleteIngredientFromRecipeCommand = new RelayCommandAsync(DeleteIngredientFromRecipe);
        } 

        private async Task GetRecipeIngredientsAsync(Product product)
        {
            try
            {
                var recipe = await _recipeService.GetRecipeByIdAsync(product.RecipeId);
                await _recipeIngredientService.GetRecipeIngredientsAsync(recipe);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się załadować listy składników, przyczyna problemu: {ex.Message}", 
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AddIngredientToRecipe()
        {
            try
            {
                var recipeIngredient = await _recipeIngredientService.CreateRecipeIngredient(selectedProduct!.Recipe, selectedIngredient!, amountOfIngredient);
                await _recipeIngredientService.AddIngredientToRecipeAsync(recipeIngredient);
                await GetRecipeIngredientsAsync(selectedProduct);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się dodać składnika do przepisu, przyczyna problemu: {ex.Message}", 
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateIngredientInRecipe()
        {
            try
            {
                var ingredient = selectedIngredient ?? selectedRecipeIngredient!.Ingredient;

                var recipeIngredient = await _recipeIngredientService.CreateRecipeIngredient(selectedProduct!.Recipe, ingredient, amountOfIngredient);
                await _recipeIngredientService.UpdateIngredientInRecipeAsync(selectedRecipeIngredient!, recipeIngredient);
                await GetRecipeIngredientsAsync(selectedProduct);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się zaktualizować składnika w przepisie, przyczyna problemu: {ex.Message}",
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteIngredientFromRecipe()
        {
            try
            {
                await _recipeIngredientService.DeleteIngredientFromRecipeAsync(selectedRecipeIngredient!);
                await GetRecipeIngredientsAsync(selectedProduct!);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się usunąć składnika z przepisu, przyczyna problemu: {ex.Message}", 
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void LoadDataIntoFormFields(object obj)
        {
            var recipeIngredient = obj as RecipeIngredient;
            if (recipeIngredient is null) throw new ArgumentNullException();

            AmountOfIngredient = recipeIngredient.Quantity.ToString();
        }

        protected override void CheckWhichButtonShouldBeEnable()
        {
            IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
            IsUpdateButtonEnable = CheckIfUpdateButtonCanBeEnabled();
            IsDeleteButtonEnable = CheckIfDeleteButtonCanBeEnabled();
        }

        protected override bool CheckIfAddButtonCanBeEnabled()
        {
            return selectedProduct is not null &&
                   selectedIngredient is not null &&
                   !amountOfIngredient.IsNullOrEmpty() &&
                   !HasErrors;
        }

        protected override bool CheckIfUpdateButtonCanBeEnabled()
        {
            return selectedProduct is not null &&
                   selectedRecipeIngredient is not null &&
                   !amountOfIngredient.IsNullOrEmpty() &&
                   !HasErrors;
        }

        protected override bool CheckIfDeleteButtonCanBeEnabled()
        {
            return selectedRecipeIngredient is not null;
        }
    }
}
