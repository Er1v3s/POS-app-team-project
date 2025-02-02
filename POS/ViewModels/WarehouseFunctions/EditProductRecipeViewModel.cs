using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Microsoft.IdentityModel.Tokens;
using POS.Services;
using POS.Services.SalesPanel;
using POS.Utilities.RelayCommands;

namespace POS.ViewModels.WarehouseFunctions
{
    public class EditProductRecipeViewModel : ProductManipulationViewModelBase
    {
        private readonly RecipeService _recipeService;
        private readonly IngredientService _ingredientService;

        private ObservableCollection<Ingredient> ingredientCollection = new();
        private ObservableCollection<RecipeIngredient> recipeIngredientCollection = new();

        private Ingredient selectedIngredient;
        private string amountOfIngredient;
        private RecipeIngredient? selectedRecipeIngredient;

        private Visibility isIngredientSelected;

        public ObservableCollection<Ingredient> IngredientCollection
        {
            get => ingredientCollection;
            set => SetField(ref ingredientCollection, value);
        }

        public ObservableCollection<RecipeIngredient> RecipeIngredientCollection
        {
            get => recipeIngredientCollection;
            set => SetField(ref recipeIngredientCollection, value);
        }

        public override Product? SelectedProduct
        {
            set
            {
                if (SetField(ref selectedProduct, value))
                {
                    IsProductSelected = Visibility.Collapsed;
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
                    _ = LoadRecipeIngredientsToCollection(value!);
                }
            }
        }

        public Ingredient SelectedIngredient
        {
            set
            {
                if (SetField(ref selectedIngredient, value))
                {
                    IsIngredientSelected = Visibility.Collapsed;
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
                }
            }
        }

        public string AmountOfIngredient
        {
            get => amountOfIngredient;
            set
            {
                if (SetField(ref amountOfIngredient, value))
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
            }
        }

        public RecipeIngredient? SelectedRecipeIngredient
        {
            get => selectedRecipeIngredient;
            set
            {
                if (SetField(ref selectedRecipeIngredient, value))
                    IsDeleteButtonEnable = CheckIfDeleteButtonCanBeEnabled();
            }
        }

        public Visibility IsIngredientSelected
        {
            get => isIngredientSelected;
            set => SetField(ref isIngredientSelected, value);
        }

        public ICommand AddIngredientToRecipeCommand { get; }
        public ICommand DeleteIngredientFromRecipeCommand { get; }

        public EditProductRecipeViewModel(
            ProductService productService,
            RecipeService recipeService,
            IngredientService ingredientService) : base (productService)
        {
            _recipeService = recipeService;
            _ingredientService = ingredientService;

            AddIngredientToRecipeCommand = new RelayCommandAsync(AddIngredientToRecipe);
            DeleteIngredientFromRecipeCommand = new RelayCommandAsync(DeleteIngredientFromRecipe);

            _ = LoadIngredientsToCollection();
        } 

        private async Task LoadRecipeIngredientsToCollection(Product product)
        {
            try
            {
                var recipe = await _recipeService.GetRecipeByIdAsync(product.RecipeId);
                var recipeIngredients = recipe.RecipeIngredients.ToList();

                LoadItemsToCollection(RecipeIngredientCollection, recipeIngredients);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się załadować listy składników, przyczyna problemu: {ex.Message}", 
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadIngredientsToCollection()
        {
            try
            {
                var ingredients = await _ingredientService.GetAllIngredientsAsync();
                LoadItemsToCollection(IngredientCollection, ingredients);
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
                await _recipeService.AddIngredientToRecipeAsync(selectedProduct!.RecipeId, selectedIngredient, amountOfIngredient);
                await LoadRecipeIngredientsToCollection(selectedProduct);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się dodać składnika do przepisu, przyczyna problemu: {ex.Message}", 
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteIngredientFromRecipe()
        {
            try
            {
                await _recipeService.DeleteIngredientFromRecipeAsync(selectedRecipeIngredient!);
                await LoadRecipeIngredientsToCollection(selectedProduct!);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się usunąć składnika z przepisu, przyczyna problemu: {ex.Message}", 
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override bool CheckIfAddButtonCanBeEnabled()
        {
            return isProductSelected == Visibility.Collapsed &&
                                isIngredientSelected == Visibility.Collapsed &&
                                !amountOfIngredient.IsNullOrEmpty();
        }

        protected override bool CheckIfDeleteButtonCanBeEnabled()
        {
            return selectedRecipeIngredient != null;
        }
    }
}
