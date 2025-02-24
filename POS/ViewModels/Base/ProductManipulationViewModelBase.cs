using System.Windows;
using DataAccess.Models;
using POS.Services.SalesPanel;
using POS.Utilities;

namespace POS.ViewModels.Base
{
    public abstract class ProductManipulationViewModelBase : ViewModelBase
    {
        protected readonly ProductService _productService;

        protected Product? selectedProduct;

        protected Visibility isProductSelected;

        private bool isDeleteButtonEnable;
        private bool isUpdateButtonEnable;
        private bool isAddButtonEnable;

        public MyObservableCollection<Product> ProductObservableCollection
        {
            get => _productService.ProductCollection;
        }

        public virtual Product? SelectedProduct
        {
            get => selectedProduct;
            set => SetField(ref selectedProduct, value);
        }

        public Visibility IsProductSelected
        {
            get => isProductSelected;
            set => SetField(ref isProductSelected, value);
        }

        public bool IsDeleteButtonEnable
        {
            get => isDeleteButtonEnable;
            set => SetField(ref isDeleteButtonEnable, value);
        }

        public bool IsUpdateButtonEnable
        {
            get => isUpdateButtonEnable;
            set => SetField(ref isUpdateButtonEnable, value);
        }
        
        public bool IsAddButtonEnable
        {
            get => isAddButtonEnable;
            set => SetField(ref isAddButtonEnable, value);
        }

        protected ProductManipulationViewModelBase(
            ProductService productService)
        {
            _productService = productService;
        }

        protected abstract bool CheckIfAddButtonCanBeEnabled();
        protected abstract bool CheckIfDeleteButtonCanBeEnabled();
        protected abstract bool CheckIfUpdateButtonCanBeEnabled();
    }
}
