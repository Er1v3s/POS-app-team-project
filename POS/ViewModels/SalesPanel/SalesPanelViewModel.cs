using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using POS.Models.Orders;
using POS.Services;
using POS.Services.Login;
using POS.Services.SalesPanel;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.SalesPanel
{
    public class SalesPanelViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private readonly ProductService _productsService;
        //private readonly OrdersService _ordersService;
        //private readonly RecipeService _recipeService;

        private const string DefaultPlaceholder = "Wpisz nazwę...";

        private string loggedInUserName;
        private string searchPhrase = DefaultPlaceholder;
        private string placeholder = DefaultPlaceholder;

        private ObservableCollection<Product> productsCollection = new();
        private ObservableCollection<OrderItemDto> orderItemsCollection = new();

        public string LoggedInUserName
        {
            get => loggedInUserName;
            set => SetField(ref loggedInUserName, value);
        }

        public string SearchPhrase
        {
            get => searchPhrase;
            set
            {
                if (SetField(ref searchPhrase, value))
                {
                    HandlePlaceholder();
                    HandleProductFiltering(value);
                }
            }
        }

        public string Placeholder
        {
            get => placeholder;
            set => SetField(ref placeholder, value);
        }

        public ObservableCollection<Product> ProductsCollection
        {
            get => productsCollection;
            set => SetField(ref productsCollection, value);
        }

        public ObservableCollection<OrderItemDto> OrderItemsCollection
        {
            get => orderItemsCollection;
            set => SetField(ref orderItemsCollection, value);
        }

        public Action CloseWindowAction;
        public ICommand MoveToMainWindowCommand { get; }
        public ICommand SelectProductCommand { get; }
        public ICommand DeleteOrderItemCommand { get; }
        public ICommand SelectCategoryCommand { get; }

        public SalesPanelViewModel(NavigationService navigationService, ProductService productsService, OrdersService ordersService)
        {
            _navigationService = navigationService;
            _productsService = productsService;
            //_ordersService = ordersService;
            //_recipeService = recipeService;

            MoveToMainWindowCommand = new RelayCommand(MoveToMainWindow);
            SelectProductCommand = new RelayCommand<Product>(AddProductToOrderItemsCollection);
            DeleteOrderItemCommand = new RelayCommand<OrderItemDto>(DeleteOrderItemFromOrderItemsCollection);
            SelectCategoryCommand = new RelayCommand<object>(FilterProductsByCategory);

            loggedInUserName = LoginManager.Instance.GetLoggedInUserFullName();

            Task.Run(LoadAllProducts);
        }

        
        private void LoadProducts(List<Product> productsList)
        {
            productsCollection.Clear();

            foreach (var product in productsList)
                 productsCollection.Add(product);
        }

        private void LoadAllProducts()
        {
            var products = _productsService.LoadAllProducts();

            LoadProducts(products);
        }

        private void FilterProductsBySearchPhrase(string searchPhraseValue)
        {
            var products = _productsService.LoadProductsBySearch(searchPhraseValue);

            LoadProducts(products);
        }

        private void FilterProductsByCategory(object categoryCommandParameter)
        {
            var products = _productsService.LoadProductsByCategory(categoryCommandParameter);

            LoadProducts(products);
        }

        private void HandlePlaceholder()
        {
            Placeholder = string.IsNullOrEmpty(SearchPhrase) || SearchPhrase == DefaultPlaceholder
                ? DefaultPlaceholder
                : string.Empty;
        }

        private void HandleProductFiltering(string searchPhraseArg)
        {
            if (string.IsNullOrEmpty(searchPhraseArg) || searchPhraseArg == DefaultPlaceholder)
                LoadAllProducts();
            else
                FilterProductsBySearchPhrase(searchPhraseArg);
        }

        private void AddProductToOrderItemsCollection(Product product)
        {
            // 1 is temporrary
            var existingProduct = orderItemsCollection.FirstOrDefault(p => p.ProductId == product.ProductId);

            if (existingProduct != null)
            {
                existingProduct.Amount++;
                // existingProduct.TotalPrice = existingProduct.Amount * existingProduct.Price;
            }
            else
            {
                orderItemsCollection.Add(new OrderItemDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Amount = 1,
                    Price = Convert.ToDouble(product.Price)
                });
            }
        }

        private void DeleteOrderItemFromOrderItemsCollection(OrderItemDto orderItem)
        {
            if (orderItem.Amount == 1)
                orderItemsCollection.Remove(orderItem);
            else
                orderItem.Amount--;
        }

        private async Task GetProductRecipe()
        {
            
        }

        private void MoveToMainWindow(object obj)
        {
            _navigationService.OpenMainWindow();

            if(Application.Current.Windows.OfType<Views.Windows.MainWindow>().Any())
                CloseWindowAction.Invoke();
        }
    }
}
