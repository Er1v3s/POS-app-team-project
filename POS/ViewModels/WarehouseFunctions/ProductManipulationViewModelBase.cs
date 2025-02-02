using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using DataAccess.Models;
using POS.Services.SalesPanel;
using POS.ViewModels.Base;

namespace POS.ViewModels.WarehouseFunctions
{
    public abstract class ProductManipulationViewModelBase : ViewModelBase
    {
        protected readonly ProductService _productService;

        protected Product? selectedProduct;

        protected Visibility isProductSelected;

        private bool isDeleteButtonEnable;
        private bool isAddButtonEnable;

        public ObservableCollection<Product> ProductObservableCollection
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

        protected void LoadItemsToCollection<T>(ObservableCollection<T> collection, List<T> items)
        {
            collection.Clear();

            foreach (var item in items)
                collection.Add(item);
        }

        protected abstract bool CheckIfAddButtonCanBeEnabled();
        protected abstract bool CheckIfDeleteButtonCanBeEnabled();
    }
}
