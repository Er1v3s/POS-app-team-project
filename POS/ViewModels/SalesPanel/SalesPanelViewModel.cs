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
using POS.Views.Windows.SalesPanel;

namespace POS.ViewModels.SalesPanel
{
    public class SalesPanelViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private readonly ProductService _productsService;
        private readonly OrderService _orderService;
        private readonly RecipeService _recipeService;

        private const string DefaultPlaceholder = "Wpisz nazwę...";

        private int currentViewIndex;

        private string loggedInUserName;
        private string searchPhrase = DefaultPlaceholder;
        private string placeholder = DefaultPlaceholder;

        private ObservableCollection<Product> productCollection = new();
        private ObservableCollection<OrderItemDto> orderItemCollection = new();
        private ObservableCollection<Recipes> recipeCollection = new();

        private double amountToPayForOrder;
        private bool isDiscountApplied;

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

        public double AmountToPayForOrder
        {
            get => amountToPayForOrder;
            set => SetField(ref amountToPayForOrder, Math.Round(value, 2));
        }

        public Action CloseWindowAction;
        public ICommand MoveToMainWindowCommand { get; }
        public ICommand SelectProductCommand { get; }
        public ICommand DeleteOrderItemCommand { get; }
        public ICommand SelectCategoryCommand { get; }
        public ICommand ShowProductCollectionCommand { get; }
        public ICommand ShowRecipeCollectionCommand { get; }
        public ICommand PayForOrderCommand { get; }
        public ICommand ApplyDiscountCommand { get; }

        public SalesPanelViewModel(
            NavigationService navigationService,
            ProductService productService,
            OrderService orderService,
            RecipeService recipeService
            )
        {
            _navigationService = navigationService;
            _productsService = productService;
            _orderService = orderService;
            _recipeService = recipeService;

            MoveToMainWindowCommand = new RelayCommand(MoveToMainWindow);
            SelectProductCommand = new RelayCommand<Product>(AddProductToOrderItemsCollection);
            DeleteOrderItemCommand = new RelayCommand<OrderItemDto>(DeleteOrderItemFromOrderItemsCollection);
            SelectCategoryCommand = new RelayCommand<object>(FilterProductsByCategory);
            ShowProductCollectionCommand = new RelayCommand(ShowProductCollectionView);
            ShowRecipeCollectionCommand = new RelayCommandAsync(ShowProductsRecipeView);
            PayForOrderCommand = new RelayCommandAsync<object>(PayForOrder);
            ApplyDiscountCommand = new RelayCommand(ApplyDiscount);

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
            SwitchViewToCollectionFromArgument(productCollection);

            var products = _productsService.LoadProductsBySearch(searchPhraseValue);
            LoadProducts(products);
        }

        private void FilterProductsByCategory(object categoryCommandParameter)
        {
            SwitchViewToCollectionFromArgument(productCollection);

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
            if (searchPhraseArg.IsNullOrEmpty() || searchPhraseArg == DefaultPlaceholder)
                ShowAllProducts();
            else
                FilterProductsBySearchPhrase(searchPhraseArg);
        }

        private void AddProductToOrderItemsCollection(Product product)
        {
            var existingProduct = orderItemCollection.FirstOrDefault(p => p.ProductId == product.ProductId);

            if (existingProduct == null)
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
            else
                existingProduct.Amount++;

            AmountToPayForOrder += Convert.ToDouble(product.Price);
        }

        private void DeleteOrderItemFromOrderItemsCollection(OrderItemDto orderItem)
        {
            if (orderItem.Amount == 1)
            {
                orderItemCollection.Remove(orderItem);

                var recipe = recipeCollection.FirstOrDefault(r => r.RecipeId == orderItem.RecipeId)!;
                recipeCollection.Remove(recipe);
            }
            else
                orderItem.Amount--;

            AmountToPayForOrder -= orderItem.Price;
        }

        private async Task PayForOrder(object paymentMethod)
        {
            var orderDto = CreateOrderDto(paymentMethod);
            var result = await _orderService.HandleTheOrder(orderDto);

            if (result)
                ClearOrder();
        }

        private OrderDto CreateOrderDto(object paymentMethod)
        {
            return new OrderDto
            {
                EmployeeId = LoginManager.Instance.Employee!.EmployeeId,
                OrderItemList = orderItemCollection.ToList(),
                AmountToPay = AmountToPayForOrder,
                PaymentMethod = paymentMethod.ToString()!
            };
        }

        private void ClearOrder()
        {
            orderItemCollection.Clear();
            recipeCollection.Clear();
            ShowProductCollectionView();
            AmountToPayForOrder = 0;
            placeholder = DefaultPlaceholder;
            isDiscountApplied = false;
        }

        private void ShowProductCollectionView()
        {
            SwitchViewToCollectionFromArgument(productCollection);
            ShowAllProducts();
        }

        private async Task ShowProductsRecipeView()
        {
            if (!orderItemCollection.IsNullOrEmpty())
            {
                recipeCollection.Clear();

                SwitchViewToCollectionFromArgument(recipeCollection);

                foreach (var product in orderItemCollection)
                {
                    var recipe = await _recipeService.GetRecipe(product);
                    recipeCollection.Add(recipe);
                }
            }
            else
                MessageBox.Show("Brak produktów do wyświetlenia przepisu");
        }

        private void ApplyDiscount()
        {
            var discountWindow = new DiscountWindow();
            discountWindow.ShowDialog();

            if (discountWindow.DialogResult == true && !isDiscountApplied)
            {
                if (!isDiscountApplied)
                {
                    if (discountWindow.radioButton10.IsChecked == true)
                        AmountToPayForOrder *= 0.9;
                    else if (discountWindow.radioButton15.IsChecked == true)
                        AmountToPayForOrder *= 0.85;

                    isDiscountApplied = true;
                }
                else
                    MessageBox.Show("Rabat został już zastosowany", "Informacja", MessageBoxButton.OK);
            }
        }

        private void SwitchViewToCollectionFromArgument<T>(ObservableCollection<T> collection)
        {
            if (collection.GetType() == typeof(ObservableCollection<Product>))
                CurrentViewIndex = 0;
            else if(collection.GetType() == typeof(ObservableCollection<Recipes>))
                CurrentViewIndex = 1;
        }

        private void MoveToMainWindow()
        {
            _navigationService.OpenMainWindow();

            if(Application.Current.Windows.OfType<Views.Windows.MainWindow>().Any())
                CloseWindowAction.Invoke();
        }
    }
}
