using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Microsoft.IdentityModel.Tokens;
using POS.Services.SalesPanel;
using POS.Utilities;
using POS.Utilities.RelayCommands;
using POS.Validators.Models;
using POS.ViewModels.Base.WarehouseFunctions;

namespace POS.ViewModels.WarehouseFunctions
{
    public class AddEditDeleteProductViewModel : FormViewModelBase
    {
        private readonly ProductService _productService;
        private readonly ProductValidator _productValidator;

        private string productName;
        private string productCategory;
        private string productPrice;
        private string productDescription;
        private string productRecipe;

        private string productNameError;
        private string productCategoryError;
        private string productPriceError;
        private string productDescriptionError;
        private string productRecipeError;

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
                    ValidateProperty(_productValidator.ValidateProductDescription, nameof(ProductRecipe), value, error => ProductRecipeError = error);
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
        public ICommand DeleteProductCommand { get; }

        public AddEditDeleteProductViewModel(ProductService productService)
        {
            _productService = productService;
            _productValidator = new ProductValidator();

            AddNewProductCommand = new RelayCommandAsync(AddNewProduct);
            DeleteProductCommand = new RelayCommandAsync(DeleteProduct);
        }

        private async Task AddNewProduct()
        {
            try
            {
                var product = await _productService.CreateProduct(productName, productCategory, productDescription, productPrice);
                await _productService.AddNewProductAsync(product);

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
            //ProductRecipe = product.Recipe;
        }

        protected override void ResetForm()
        {
            ProductName = string.Empty;
            ProductCategory = string.Empty;
            ProductPrice = string.Empty;
            ProductDescription = string.Empty;
            ProductRecipe = string.Empty;

            SelectedItem = null;
            IsItemSelected = Visibility.Visible;
        }

        protected override void ClearNameField()
        {
            ProductName = string.Empty;
            ProductNameError = string.Empty;
        }

        protected override bool CheckIfAddButtonCanBeEnabled()
        {
            return IsNewItem &&
                   !productName.IsNullOrEmpty() &&
                   !productCategory.IsNullOrEmpty() &&
                   !productPrice.IsNullOrEmpty() &&
                   !productDescription.IsNullOrEmpty() &&
                   !productRecipe.IsNullOrEmpty();

        }

        protected override bool CheckIfUpdateButtonCanBeEnabled()
        {
            return SelectedItem != null &&
                   !productName.IsNullOrEmpty() &&
                   !productCategory.IsNullOrEmpty() &&
                   !productPrice.IsNullOrEmpty() &&
                   !productDescription.IsNullOrEmpty() &&
                   !productRecipe.IsNullOrEmpty() &&
                   !HasErrors;
        }

        protected override bool CheckIfDeleteButtonCanBeEnabled()
        {
            return SelectedItem != null;
        }
    }
}
