using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Microsoft.IdentityModel.Tokens;
using POS.Services.SalesPanel;
using POS.Utilities.RelayCommands;

namespace POS.ViewModels.WarehouseFunctions
{
    public class AddEditDeleteProductViewModel : ProductManipulationViewModelBase
    {
        private string productName;
        private string productCategory;
        private string productPrice;
        private string productDescription;
        private string productRecipe;

        private bool isNewProduct;

        public override Product? SelectedProduct
        {
            get => selectedProduct;
            set
            {
                if (SetField(ref selectedProduct, value))
                {
                    IsProductSelected = Visibility.Collapsed;

                    if (isNewProduct && selectedProduct != null)
                        IsNewProduct = false;

                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
                    IsDeleteButtonEnable = CheckIfDeleteButtonCanBeEnabled();
                }
            }
        }

        public string ProductName
        {
            get => productName;
            set
            {
                if(SetField(ref productName, value))
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
            }
        }

        public string ProductCategory
        {
            get => productCategory;
            set
            {
                if (SetField(ref productCategory, value))
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
            }
        }

        public string ProductPrice
        {
            get => productPrice;
            set
            {
                if (SetField(ref productPrice, value))
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
            }
        }

        public string ProductDescription
        {
            get => productDescription;
            set
            {
                if (SetField(ref productDescription, value))
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
            }
        }

        public string ProductRecipe
        {
            get => productRecipe;
            set
            {
                if (SetField(ref productRecipe, value))
                    IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
            }
        }

        public bool IsNewProduct
        {
            get => isNewProduct;
            set
            {
                if (SetField(ref isNewProduct, value))
                {
                    if (value)
                    {
                        SelectedProduct = null;
                        IsProductSelected = Visibility.Visible;
                        IsAddButtonEnable = CheckIfAddButtonCanBeEnabled();
                    }
                    else
                        ProductName = string.Empty;
                }
            }
        }

        public ICommand AddNewProductCommand { get; }
        public ICommand DeleteProductCommand { get; }

        public AddEditDeleteProductViewModel(ProductService productService) : base(productService)
        {
            AddNewProductCommand = new RelayCommandAsync(AddNewProduct);
            DeleteProductCommand = new RelayCommandAsync(DeleteProduct);
        }

        private async Task AddNewProduct()
        {
            try
            {
                var product = CreateProduct();
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
                await _productService.DeleteProductAsync(selectedProduct!);

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

        private void ResetForm()
        {
            ProductName = string.Empty;
            ProductCategory = string.Empty;
            ProductPrice = string.Empty;
            ProductDescription = string.Empty;
            ProductRecipe = string.Empty;

            SelectedProduct = null;
            IsProductSelected = Visibility.Visible;
        }

        private Product CreateProduct()
        {
            return new Product
            {
                ProductName = productName,
                Category = productCategory,
                Description = productDescription,
                Price = double.Parse(productPrice),
            };
        }

        protected override bool CheckIfAddButtonCanBeEnabled()
        {
            return isNewProduct &&
                   !productName.IsNullOrEmpty() &&
                   !productCategory.IsNullOrEmpty() &&
                   !productPrice.IsNullOrEmpty() &&
                   !productDescription.IsNullOrEmpty() &&
                   !productRecipe.IsNullOrEmpty();

        }

        protected override bool CheckIfDeleteButtonCanBeEnabled()
        {
            return selectedProduct != null;
        }
    }
}
