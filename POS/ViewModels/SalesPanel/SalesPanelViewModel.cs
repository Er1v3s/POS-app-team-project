using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Microsoft.IdentityModel.Tokens;
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
        private readonly RecipeService _recipeService;

        private const string DefaultPlaceholder = "Wpisz nazwę...";

        private int currentViewIndex;

        private string loggedInUserName;
        private string searchPhrase = DefaultPlaceholder;
        private string placeholder = DefaultPlaceholder;

        private ObservableCollection<Product> productCollection = new();
        private ObservableCollection<OrderItemDto> orderItemCollection = new();
        private ObservableCollection<Recipes> recipeCollection = new();

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

        public ObservableCollection<Product> ProductCollection
        {
            get => productCollection;
            set => SetField(ref productCollection, value);
        }

        public ObservableCollection<OrderItemDto> OrderItemCollection
        {
            get => orderItemCollection;
            set => SetField(ref orderItemCollection, value);
        }

        public ObservableCollection<Recipes> RecipeCollection
        {
            get => recipeCollection;
            set => SetField(ref recipeCollection, value);
        }

        public int CurrentViewIndex
        {
            get => currentViewIndex;
            set => SetField(ref currentViewIndex, value);
        }

        public Action CloseWindowAction;
        public ICommand MoveToMainWindowCommand { get; }
        public ICommand SelectProductCommand { get; }
        public ICommand DeleteOrderItemCommand { get; }
        public ICommand SelectCategoryCommand { get; }
        public ICommand ShowProductCollectionCommand { get; }
        public ICommand ShowRecipeCollectionCommand { get; }

        public SalesPanelViewModel(
            NavigationService navigationService,
            ProductService productService,
            OrdersService ordersService,
            RecipeService recipeService
            )
        {
            _navigationService = navigationService;
            _productsService = productService;
            //_ordersService = ordersService;
            _recipeService = recipeService;

            MoveToMainWindowCommand = new RelayCommand(MoveToMainWindow);
            SelectProductCommand = new RelayCommand<Product>(AddProductToOrderItemsCollection);
            DeleteOrderItemCommand = new RelayCommand<OrderItemDto>(DeleteOrderItemFromOrderItemsCollection);
            SelectCategoryCommand = new RelayCommand<object>(FilterProductsByCategory);
            ShowProductCollectionCommand = new RelayCommand(ShowProductCollectionView);
            ShowRecipeCollectionCommand = new RelayCommandAsync(ShowProductsRecipeView);

            loggedInUserName = LoginManager.Instance.GetLoggedInUserFullName();

            ShowAllProducts();
        }

        private void LoadProducts(List<Product> productsList)
        {
            productCollection.Clear();

            foreach (var product in productsList)
                 productCollection.Add(product);
        }

        private void ShowAllProducts()
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
                ShowAllProducts();
            else
                FilterProductsBySearchPhrase(searchPhraseArg);
        }

        private void AddProductToOrderItemsCollection(Product product)
        {
            // 1 is temporrary
            var existingProduct = orderItemCollection.FirstOrDefault(p => p.ProductId == product.ProductId);

            if (existingProduct != null)
            {
                existingProduct.Amount++;
                // existingProduct.TotalPrice = existingProduct.Amount * existingProduct.Price;
            }
            else
            {
                orderItemCollection.Add(new OrderItemDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    RecipeId = product.RecipeId,
                    Amount = 1,
                    Price = Convert.ToDouble(product.Price)
                });
            }
        }

        private void DeleteOrderItemFromOrderItemsCollection(OrderItemDto orderItem)
        {
            if (orderItem.Amount == 1)
                orderItemCollection.Remove(orderItem);
            else
                orderItem.Amount--;
        }

        private void ShowProductCollectionView()
        {
            SwitchView();
        }

        private async Task ShowProductsRecipeView()
        {
            if (orderItemCollection.IsNullOrEmpty())
            {
                MessageBox.Show("Brak produktów do wyświetlenia przepisu");
            }
            else
            {
                recipeCollection.Clear();

                SwitchView();

                foreach (var product in orderItemCollection)
                {
                    var recipe = await _recipeService.GetRecipe(product);

                    recipeCollection.Add(recipe);
                }
            }
        }

        private void SwitchView()
        {
            CurrentViewIndex = CurrentViewIndex == 0 ? 1 : 0;
        }

        private void MoveToMainWindow()
        {
            _navigationService.OpenMainWindow();

            if(Application.Current.Windows.OfType<Views.Windows.MainWindow>().Any())
                CloseWindowAction.Invoke();
        }
    }
}
