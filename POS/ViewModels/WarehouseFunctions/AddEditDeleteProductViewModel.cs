using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using POS.Services;
using POS.Utilities;
using POS.Utilities.RelayCommands;
using POS.Validators.Models;
using POS.ViewModels.Base.WarehouseFunctions;

namespace POS.ViewModels.WarehouseFunctions
{
    public class AddEditDeleteProductViewModel : FormViewModelBase
    {
        private readonly ProductService _productService;
        private readonly RecipeService _recipeService;

        private readonly ProductValidator _productValidator;
        private readonly RecipeValidator _recipeValidator;

        private string productName = string.Empty;
        private string productCategory = string.Empty;
        private string productPrice = string.Empty;
        private string productDescription = string.Empty;
        private string productRecipe = string.Empty;

        private string productNameError = string.Empty;
        private string productCategoryError = string.Empty;
        private string productPriceError = string.Empty;
        private string productDescriptionError = string.Empty;
        private string productRecipeError = string.Empty;

        public MyObservableCollection<Product> ProductObservableCollection => _productService.ProductCollection;

        public string ProductName
        {
            get => productName;
            set
            {
                if (SetField(ref productName, value))
                    ValidateProperty(_productValidator.ValidateProductName, nameof(ProductName), value, error => ProductNameError = error);
            }
        }

        public string ProductCategory
        {
            get => productCategory;
            set
            {
                if (SetField(ref productCategory, value))
                    ValidateProperty(_productValidator.ValidateProductCategory, nameof(ProductCategory), value, error => ProductCategoryError = error);
            }
        }

        public string ProductPrice
        {
            get => productPrice;
            set
            {
                if (SetField(ref productPrice, value))
                    ValidateProperty(_productValidator.ValidateProductPrice, nameof(ProductPrice), value, error => ProductPriceError = error);
            }
        }

        public string ProductDescription
        {
            get => productDescription;
            set
            {
                if (SetField(ref productDescription, value))
                    ValidateProperty(_productValidator.ValidateProductDescription, nameof(ProductDescription), value, error => ProductDescriptionError = error);
            }
        }

        public string ProductRecipe
        {
            get => productRecipe;
            set
            {
                if (SetField(ref productRecipe, value))
                    ValidateProperty(_recipeValidator.ValidateRecipeContent, nameof(ProductRecipe), value, error => ProductRecipeError = error);
            }
        }

        public string ProductNameError
        {
            get => productNameError;
            set => SetField(ref productNameError, value);
        }

        public string ProductCategoryError
        {
            get => productCategoryError;
            set => SetField(ref productCategoryError, value);
        }

        public string ProductPriceError
        {
            get => productPriceError;
            set => SetField(ref productPriceError, value);
        }

        public string ProductDescriptionError
        {
            get => productDescriptionError;
            set => SetField(ref productDescriptionError, value);
        }

        public string ProductRecipeError
        {
            get => productRecipeError;
            set => SetField(ref productRecipeError, value);
        }

        public ICommand AddNewProductCommand { get; }
        public ICommand UpdateProductCommand { get; }
        public ICommand DeleteProductCommand { get; }

        public AddEditDeleteProductViewModel(ProductService productService, RecipeService recipeService)
        {
            _productService = productService;
            _recipeService = recipeService;
            _productValidator = new ProductValidator();
            _recipeValidator = new RecipeValidator();

            AddNewProductCommand = new RelayCommandAsync(AddNewProduct);
            UpdateProductCommand = new RelayCommandAsync(UpdateProduct);
            DeleteProductCommand = new RelayCommandAsync(DeleteProduct);
        }

        private async Task AddNewProduct()
        {
            try
            {
                var newRecipe = await _recipeService.CreateRecipe(productName, productRecipe);
                var newProduct = await _productService.CreateProduct(productName, productCategory, productDescription, productPrice, newRecipe);
                await _productService.AddNewProductAsync(newProduct);

                MessageBox.Show("Pomyślnie dodano nowy produkt", 
                    "Informacja", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się utworzyć produktu, przyczyna problemu: {ex.Message}",
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateProduct()
        {
            try
            {
                var updatedRecipe = await _recipeService.CreateRecipe(productName, productRecipe);
                var updatedProduct = await _productService.CreateProduct(productName, productCategory, productDescription, productPrice, updatedRecipe);
                await _productService.UpdateExistingProductAsync((SelectedItem as Product)!, updatedProduct);

                MessageBox.Show("Pomyślnie zaktualizowano produkt",
                    "Informacja", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się utworzyć produktu, przyczyna problemu: {ex.Message}",
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteProduct()
        {
            try
            {
                await _productService.DeleteProductAsync((SelectedItem as Product)!);

                MessageBox.Show("Pomyślnie usunięto produkt",
                    "Informacja", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się usunąć produktu, przyczyna problemu: {ex.Message}",
                    "Wystąpił nieoczekiwany problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void LoadDataIntoFormFields(object obj)
        {
            var product = obj as Product;
            if (product is null) throw new ArgumentNullException();

            ProductName = product.ProductName;
            ProductCategory = product.Category;
            ProductDescription = product.Description;
            ProductPrice = product.Price.ToString();
            ProductRecipe = product.Recipe.RecipeContent;
        }
    }
}
