using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using Microsoft.IdentityModel.Tokens;
using POS.Models.Invoices;
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
        private readonly DiscountService _discountService;
        private readonly InvoiceService _invoiceService;
        private readonly IngredientService _ingredientService;

        private const string DefaultPlaceholder = "Wpisz nazwę...";

        private int currentViewIndex;

        private string loggedInUserName;
        private string searchPhrase = DefaultPlaceholder;
        private string placeholder = DefaultPlaceholder;

        private ObservableCollection<Product> productCollection = new();
        private ObservableCollection<OrderItemDto> orderItemCollection = new();
        private ObservableCollection<OrderDto> orderCollection = new ();
        private ObservableCollection<Recipe> recipeCollection = new();

        private double amountToPayForOrder;
        private double tempAmountToPayForOrder;
        private int discountValue;
        private InvoiceDto? invoiceCustomerData;

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

        public ObservableCollection<OrderDto> OrderCollection
        {
            get => orderCollection;
            set => SetField(ref orderCollection, value);
        }

        public ObservableCollection<Recipe> RecipeCollection
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

        public int DiscountValue
        {
            get => discountValue;
            set => SetField(ref discountValue, value);
        }

        public ICommand MoveToMainWindowCommand { get; }
        public ICommand SelectProductCommand { get; }
        public ICommand DeleteOrderItemCommand { get; }
        public ICommand SelectCategoryCommand { get; }
        public ICommand ShowProductCollectionCommand { get; }
        public ICommand ShowRecipeCollectionCommand { get; }
        public ICommand CancelOrderCommand { get; }
        public ICommand PayForOrderCommand { get; }
        public ICommand ApplyDiscountCommand { get; }
        public ICommand ShowSavedOrdersCommand { get; }
        public ICommand LoadOrderCommand { get; }
        public ICommand ShowFinishedOrdersCommand { get; }
        public ICommand AddInvoiceCommand { get; }

        public SalesPanelViewModel(
            NavigationService navigationService,
            ProductService productService,
            OrderService orderService,
            RecipeService recipeService,
            DiscountService discountService,
            InvoiceService invoiceService,
            IngredientService ingredientService
            )
        {
            _navigationService = navigationService;
            _productsService = productService;
            _orderService = orderService;
            _recipeService = recipeService;
            _discountService = discountService;
            _invoiceService = invoiceService;
            _ingredientService = ingredientService;

            MoveToMainWindowCommand = new RelayCommand(MoveToMainWindow);
            SelectProductCommand = new RelayCommand<Product>(AddProductToOrderItemsCollection);
            DeleteOrderItemCommand = new RelayCommand<OrderItemDto>(DeleteOrderItemFromOrderItemsCollection);
            SelectCategoryCommand = new RelayCommand<object>(FilterProductsByCategory);
            ShowProductCollectionCommand = new RelayCommand(ShowProductCollectionView);
            ShowRecipeCollectionCommand = new RelayCommandAsync(ShowProductsRecipeView);
            CancelOrderCommand = new RelayCommand(CancelOrder);
            PayForOrderCommand = new RelayCommandAsync<string>(PayForOrder);
            ApplyDiscountCommand = new RelayCommand(ApplyDiscount);
            ShowSavedOrdersCommand = new RelayCommand(ShowSavedOrdersView);
            LoadOrderCommand = new RelayCommand<OrderDto>(LoadSavedOrder);
            ShowFinishedOrdersCommand = new RelayCommand(ShowFinishedOrders);
            AddInvoiceCommand = new RelayCommand(AddInvoice);

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
            Placeholder = SearchPhrase.IsNullOrEmpty() || SearchPhrase == DefaultPlaceholder
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

            tempAmountToPayForOrder += Convert.ToDouble(product.Price);
            RecalculateAmountToPay();
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

            tempAmountToPayForOrder -= orderItem.Price;
            RecalculateAmountToPay();
        }

        private async Task PayForOrder(string paymentMethod)
        {
            var orderDto = CreateOrderDto(paymentMethod);

            var summaryOrderWindow = new OrderSummaryWindow(orderDto);
            summaryOrderWindow.ShowDialog();

            if (summaryOrderWindow.DialogResult == true)
            {
                try
                {
                    await _orderService.HandleOrderAsync(orderDto);
                    ClearOrder();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
                MessageBox.Show("Zamówienie jest puste, brak możliwości dokanania płatności", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private OrderDto CreateOrderDto(string paymentMethod = "")
        {
            return new OrderDto
            {
                EmployeeId = LoginManager.Instance.Employee!.EmployeeId,
                OrderItemList = orderItemCollection.ToList(),
                AmountToPay = amountToPayForOrder,
                PaymentMethod = paymentMethod,
                Discount = discountValue,
                InvoiceData = invoiceCustomerData,
            };
        }

        private void SaveOrder()
        {
            var orderDto = CreateOrderDto();
            OrderCollection.Add(orderDto);
            ClearOrder();
        }

        private void CancelOrder()
        {
            var result = MessageBox.Show("Anulować zamówienie?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ClearOrder();

                if(!orderCollection.Any())
                    SwitchViewToCollectionFromArgument(productCollection);
            }
        }

        private void ApplyDiscount()
        {
            DiscountWindow discountWindow = new();
            discountWindow.ShowDialog();

            if (discountWindow.DialogResult == true)
            {
                DiscountValue = _discountService.DiscountValue;
                RecalculateAmountToPay();
            }
        }
        
        private void AddInvoice()
        {
            InvoiceWindow invoiceWindow = new();
            invoiceWindow.ShowDialog();

            if (invoiceWindow.DialogResult == true)
                invoiceCustomerData = _invoiceService.GetInvoiceCustomerData();
        }

        private void ClearOrder()
        {
            orderItemCollection.Clear();
            recipeCollection.Clear();
            AmountToPayForOrder = 0;
            DiscountValue = 0;
            tempAmountToPayForOrder = 0;
            placeholder = DefaultPlaceholder;
            invoiceCustomerData = null;
            RecalculateAmountToPay();
        }

        private void RecalculateAmountToPay()
        {
            if (DiscountValue > 0)
                AmountToPayForOrder = tempAmountToPayForOrder * (1 - DiscountValue / 100d);
            else
                AmountToPayForOrder = tempAmountToPayForOrder;
        }

        private void LoadSavedOrder(OrderDto orderDto)
        {
            foreach (var dto in orderDto.OrderItemList)
            {
                OrderItemCollection.Add(dto);
                tempAmountToPayForOrder += dto.Price;
            }

            DiscountValue = orderDto.Discount;

            OrderCollection.Remove(orderDto);
            RecalculateAmountToPay();
            SwitchViewToCollectionFromArgument(productCollection);
        }

        private void ShowFinishedOrders()
        {
            _orderService.LoadFinishedOrdersWindow();
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

        private void ShowSavedOrdersView()
        {
            if (orderItemCollection.Any())
            {
                SaveOrder();
                SwitchViewToCollectionFromArgument(orderCollection);
            }
            else
            {
                if (orderCollection.Any())
                    SwitchViewToCollectionFromArgument(orderCollection);
                else
                    MessageBox.Show("Brak zamówień do wyświetlenia");
            }
        }

        private void SwitchViewToCollectionFromArgument<T>(ObservableCollection<T> collection)
        {
            if (collection.GetType() == typeof(ObservableCollection<Product>))
                CurrentViewIndex = 0;
            else if(collection.GetType() == typeof(ObservableCollection<Recipe>))
                CurrentViewIndex = 1;
            else if (collection.GetType() == typeof(ObservableCollection<OrderDto>))
                CurrentViewIndex = 2;
        }

        private void MoveToMainWindow()
        {
            _navigationService.OpenMainWindow();

            if(Application.Current.Windows.OfType<Views.Windows.MainWindow>().Any())
                CloseWindowBaseAction!.Invoke();
        }
    }
}
