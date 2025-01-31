using System;
using System.Collections.Generic;
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
using POS.ViewModels.Base;

namespace POS.ViewModels.WarehouseFunctions
{
    public class EditProductRecipeViewModel : ViewModelBase
    {
        private readonly ProductService _productService;
        private readonly RecipeService _recipeService;
        private readonly IngredientService _ingredientService;

        private ObservableCollection<Product> productCollection = new();
        private ObservableCollection<Ingredient> ingredientCollection = new();
        private ObservableCollection<RecipeIngredient> recipeIngredientCollection = new();

        private Product selectedProduct;
        private Ingredient selectedIngredient;
        private string amountOfIngredient;
        private RecipeIngredient? selectedRecipeIngredient;

        private Visibility isProductSelected;
        private Visibility isIngredientSelected;

        private bool isDeleteButtonEnable;
        private bool isAddButtonEnable;

        public ObservableCollection<Product> ProductCollection
        {
            get => productCollection;
            set => SetField(ref productCollection, value);
        }

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

        public Product SelectedProduct
        {
            get => selectedProduct;
            set
            {
                if (SetField(ref selectedProduct, value))
                {
                    IsProductSelected = Visibility.Collapsed;
                    CheckIfAddButtonCanBeEnabled();
                    _ = LoadRecipeIngredientsToCollection(value);
                }
            }
        }

        public Ingredient SelectedIngredient
        {
            get => selectedIngredient;
            set
            {
                if (SetField(ref selectedIngredient, value))
                {
                    IsIngredientSelected = Visibility.Collapsed;
                    CheckIfAddButtonCanBeEnabled();
                }
            }
        }

        public string AmountOfIngredient
        {
            get => amountOfIngredient;
            set
            {
                if (SetField(ref amountOfIngredient, value))
                {
                    CheckIfAddButtonCanBeEnabled();
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
                    CheckIfDeleteButtonCanBeEnabled();
                }
            }
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
        
        public bool IsDeleteButtonEnable
        {
            get => isDeleteButtonEnable;
            set => SetField(ref isDeleteButtonEnable, value);
        }
        
        public bool IsAddButtonEnable
        {
            get => isAddButtonEnable;
            set => SetField(ref isAddButtonEnable, value);
        }

        public ICommand AddIngredientToRecipeCommand { get; }
        public ICommand DeleteIngredientFromRecipeCommand { get; }

        public EditProductRecipeViewModel(
            ProductService productService,
            RecipeService recipeService,
            IngredientService ingredientService)
        {
            _productService = productService;
            _recipeService = recipeService;
            _ingredientService = ingredientService;

            AddIngredientToRecipeCommand = new RelayCommandAsync(AddIngredientToRecipe);
            DeleteIngredientFromRecipeCommand = new RelayCommandAsync(DeleteIngredientFromRecipe);

            LoadProductsToCollection();
            _ = LoadIngredientsToCollection();
        }

        private void LoadItemsToCollection<T>(ObservableCollection<T> collection, List<T> items)
        {
            collection.Clear();

            foreach (var item in items)
                collection.Add(item);
        }

        private void LoadProductsToCollection()
        {
            try
            {
                var products = _productService.LoadAllProducts();
                LoadItemsToCollection(ProductCollection, products);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się załadować listy produktów, powód: {ex.Message}",
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                MessageBox.Show($"Nie udało się załadować listy składników, powód: {ex.Message}", 
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
                MessageBox.Show($"Nie udało się załadować listy składników, powód: {ex.Message}",
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AddIngredientToRecipe()
        {
            try
            {
                await _recipeService.AddIngredientToRecipeAsync(selectedProduct.RecipeId, selectedIngredient, amountOfIngredient);
                await LoadRecipeIngredientsToCollection(selectedProduct);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się dodać składnika do przepisu, powód: {ex.Message}", 
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteIngredientFromRecipe()
        {
            try
            {
                await _recipeService.DeleteIngredientFromRecipeAsync(selectedRecipeIngredient!);
                await LoadRecipeIngredientsToCollection(selectedProduct);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się usunąć składnika z przepisu, powód: {ex.Message}", 
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckIfAddButtonCanBeEnabled()
        {
            IsAddButtonEnable = isProductSelected == Visibility.Collapsed &&
                                isIngredientSelected == Visibility.Collapsed &&
                                !amountOfIngredient.IsNullOrEmpty();
        }

        private void CheckIfDeleteButtonCanBeEnabled()
        {
            IsDeleteButtonEnable = selectedRecipeIngredient != null;
        }
    }
}
