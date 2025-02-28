﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Microsoft.IdentityModel.Tokens;
using POS.Services;
using POS.Services.SalesPanel;
using POS.Utilities;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base.WarehouseFunctions;

namespace POS.ViewModels.WarehouseFunctions
{
    public class EditProductRecipeViewModel : FormViewModelBase
    {
        private readonly ProductService _productService;
        private readonly IngredientService _ingredientService;
        private readonly RecipeService _recipeService;
        private readonly RecipeIngredientService _recipeIngredientService;

        private MyObservableCollection<RecipeIngredient> recipeIngredientCollection = new();

        private Product? selectedProduct;
        private Ingredient selectedIngredient;
        private RecipeIngredient? selectedRecipeIngredient;

        private string amountOfIngredient;
        private string amountOfIngredientError;

        private Visibility isProductSelected;
        private Visibility isIngredientSelected;

        public MyObservableCollection<Product> ProductObservableCollection => _productService.ProductCollection;

        public MyObservableCollection<RecipeIngredient> RecipeIngredientCollection
        {
            get => recipeIngredientCollection;
            set => SetField(ref recipeIngredientCollection, value);
        }

        public MyObservableCollection<Ingredient> IngredientObservableCollection => _ingredientService.IngredientCollection;

        public Product? SelectedProduct
        {
            set
            {
                if (SetField(ref selectedProduct, value))
                {
                    IsProductSelected = Visibility.Collapsed;
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
                    _ = GetRecipeIngredientsAsync(value!);
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

        public RecipeIngredient? SelectedRecipeIngredient
        {
            get => selectedRecipeIngredient;
            set
            {
                if (SetField(ref selectedRecipeIngredient, value))
                    IsDeleteButtonEnable = CheckIfDeleteButtonCanBeEnabled();
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
        
        public string AmountOfIngredientError
        {
            get => amountOfIngredientError;
            set
            {
                if (SetField(ref amountOfIngredientError, value))
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
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

            AddIngredientToRecipeCommand = new RelayCommandAsync(AddIngredientToRecipe);
            UpdateIngredientInRecipeCommand = new RelayCommandAsync(UpdateIngredientInRecipe);
            DeleteIngredientFromRecipeCommand = new RelayCommandAsync(DeleteIngredientFromRecipe);
        } 

        private async Task GetRecipeIngredientsAsync(Product product)
        {
            try
            {
                var recipe = await _recipeService.GetRecipeByIdAsync(product.RecipeId);
                var recipeIngredients = recipe.RecipeIngredients.ToList();

                RecipeIngredientCollection.Clear();
                RecipeIngredientCollection.AddRange(recipeIngredients);
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
                var recipeIngredient = await _recipeIngredientService.CreateRecipeIngredient(selectedProduct!.Recipe, selectedIngredient, amountOfIngredient);
                await _recipeService.AddIngredientToRecipeAsync(recipeIngredient);
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
                var recipeIngredient = await _recipeIngredientService.CreateRecipeIngredient(selectedProduct!.Recipe, selectedIngredient, amountOfIngredient);
                await _recipeService.UpdateIngredientInRecipeAsync(selectedRecipeIngredient!, recipeIngredient);
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
                await _recipeService.DeleteIngredientFromRecipeAsync(selectedRecipeIngredient!);
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
            throw new NotImplementedException();
        }

        protected override void ResetForm()
        {
            throw new NotImplementedException();
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

        protected override bool CheckIfUpdateButtonCanBeEnabled()
        {
            throw new NotImplementedException();
        }
    }
}
