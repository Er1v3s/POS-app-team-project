using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DataAccess;
using DataAccess.Models;
using Microsoft.Extensions.DependencyInjection;
using POS.Helpers;
using POS.Models.Invoices;
using POS.Models.Orders;
using POS.ViewModels.SalesPanel;

namespace POS.Views.Windows.SalesPanel
{
    /// <summary>
    /// Logika interakcji dla klasy SalesPanel.xaml
    /// </summary>
    public partial class SalesPanelWindow : Window
    {
        //private Employees currentUser;
        public int EmployeeId;
        private double totalPrice = 0;
        private int currentOrderId = 0;
        private bool discountApplied = false;

        ObservableCollection<OrderItemDto> orderList = [];
        ObservableCollection<ObservableCollection<OrderItemDto>> orderListCollection = [];

        public SalesPanelWindow(int employeeId)
        {
            orderListCollection.Add(orderList);

            //
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<SalesPanelViewModel>();

            var viewModel = (SalesPanelViewModel)DataContext;
            viewModel.CloseWindowAction = Close;
            //

            //LoadAllProducts();
            //orderListDataGrid.ItemsSource = orderListCollection[currentOrderId];
            //UpdateTotalPrice();
            EmployeeId = employeeId;
        }

        private void PayForOrder_ButtonClick(object sender, RoutedEventArgs e)
        {
            //double totalPrice = Math.Round(orderList.Sum(item => item.Amount * item.Price), 2);

            //if (sender is Button button && button.Tag is string paymentMethod)
            //{
            //    var summaryOrderWindow = new OrderSummaryWindow(orderList);
            //    summaryOrderWindow.ShowDialog();

            //    if(summaryOrderWindow.DialogResult == true)
            //    {
            //        var order = SaveOrder();
            //        SaveOrderItems(order);
            //        SavePayment(order, paymentMethod, totalPrice);
            //        RemoveIngredients();
            //        orderList.Clear();
            //        UpdateTotalPrice();
            //        MessageBox.Show($"Zapłacono za zamówienie {totalPrice:C} - metoda płatności: {paymentMethod}");
            //    } 
            //}
        }

        private void RemoveIngredients()
        {
            using (var dbContext = new AppDbContext())
            {
                foreach (var item in orderList)
                {
                    var recipeId = dbContext.Product
                                            .Where(p => p.ProductId == item.ProductId)
                                            .Select(p => p.RecipeId)
                                            .FirstOrDefault();
                    if (recipeId == null)
                    {
                        throw new Exception("Nie znaleziono przepisu dla danego produktu");
                    }

                    var recipeIngredientsId = dbContext.RecipeIngredients
                            .Where(ri => ri.RecipeId == recipeId)
                            .Select(ri => ri.IngredientId)
                            .ToList();

                    foreach(var ingredientId in recipeIngredientsId)
                    {
                        var ingredient = dbContext.Ingredients
                                        .Where(i => i.IngredientId == ingredientId)
                                        .FirstOrDefault();

                        var recipeIngredient = dbContext.RecipeIngredients
                                                .Where(ri => ri.IngredientId == ingredientId)
                                                .Select(ri => ri.Quantity)
                                                .FirstOrDefault();

                        if (ingredient != null)
                        {
                            // Tak funkcja prezentuje się w poprawny sposób, ale trzeba zmienić sposób przedstawiania ilości składników w bazie danych 
                            //ingredient.Stock -= (int)recipeIngredient;

                            // Tymczasowe, po zmianie wartości w bazie danych usunąć!!! 
                            ingredient.Stock -= 1;
                        }
                        else
                        {
                            throw new Exception("Składnik nie znaleziony w magazynie.");
                        }
                    }
                    
                }

                dbContext.SaveChanges();
            }
        }

        private void ShowOpenOrders_ButtonClick(object sender, RoutedEventArgs e)
        {
            //ProductsUnifromGrid.Children.Clear();
            //ProductsUnifromGrid.Columns = 5;

            //CreateNewOrderButton();

            //for (int i = 0; i < orderListCollection.Count; i++)
            //{
            //    int orderId = i;
            //    double totalPriceForOrder = CalculateTotalPriceForOrder(orderListCollection[orderId]);

            //    CreateOrderButton(orderId, totalPriceForOrder);
            //}
        }

        //private double CalculateTotalPriceForOrder(ObservableCollection<OrderItemDto> order)
        //{
        //    return order.Sum(item => item.Amount * item.Price);
        //}

        private void DeleteCurrentOrder_ButtonClick(object sender, RoutedEventArgs e)
        {
            //MessageBoxResult result = MessageBox.Show("Spowoduje to utratę aktualnie wyświetlonego zamówienia.\n Czy chcesz kontynuować?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            //if (result == MessageBoxResult.Yes)
            //{
            //    if (orderListCollection.Count > 1)
            //    {
            //        removeOrder();
            //    }
            //    else
            //    {
            //        ObservableCollection<OrderItemDto> newOrder = new ObservableCollection<OrderItemDto>();
            //        orderListCollection.Add(newOrder);
            //        removeOrder();
            //    }
            //}
        }

        //private Button CreateNewOrderButton()
        //{
        //    Viewbox viewbox = new Viewbox();
        //    Button newOrderButton = new Button
        //    {
        //        Style = (Style)FindResource("createNewOrderButton"),
        //        Content = "Nowe zamówienie"
        //    };

        //    newOrderButton.Click += (object sender, RoutedEventArgs e) =>
        //    {
        //        ObservableCollection<OrderItemDto> newOrder = new ObservableCollection<OrderItemDto>();
        //        orderListCollection.Add(newOrder);
        //        currentOrderId = orderListCollection.Count - 1;
        //        orderListDataGrid.ItemsSource = orderListCollection[currentOrderId];
        //        UpdateTotalPrice();
        //        ShowRecipes_ButtonClick(null, null);
        //        LoadAllProducts();
        //    };

        //    ProductsUnifromGrid.Children.Add(viewbox);
        //    viewbox.Child = newOrderButton;
        //    return newOrderButton;
        //}

        //private Button CreateOrderButton(int orderId, double totalPriceForOrder)
        //{
        //    Viewbox viewbox = new Viewbox();
        //    Button orderButton = new Button
        //    {
        //        Style = (Style)FindResource("chooseProductButton"),
        //        Content = new StackPanel
        //        {
        //            Children =
        //            {
        //                new TextBlock { TextAlignment = TextAlignment.Center, Margin = new Thickness(10), Text = $"Zamówienie {orderId + 1}" },
        //                new TextBlock { TextAlignment = TextAlignment.Center, Margin = new Thickness(10), Text = $"Suma: {totalPriceForOrder:C2}" }
        //            }
        //        }
        //    };

        //    orderButton.Click += (object sender, RoutedEventArgs e) =>
        //    {
        //        currentOrderId = orderId;
        //        orderListDataGrid.ItemsSource = orderListCollection[currentOrderId];
        //        UpdateTotalPrice();
        //        ShowRecipes_ButtonClick(null, null);
        //        LoadAllProducts();
        //    };

        //    ProductsUnifromGrid.Children.Add(viewbox);
        //    viewbox.Child = orderButton;
        //    return orderButton;
        //}

        //private void removeOrder()
        //{
        //    orderListCollection.RemoveAt(currentOrderId);
        //    currentOrderId = 0;
        //    orderListDataGrid.ItemsSource = orderListCollection[currentOrderId];
        //    UpdateTotalPrice();
        //    LoadAllProducts();

        //    discountApplied = false;
        //}
        
        //private void ApplyDiscount_ButtonClick(object sender, RoutedEventArgs e)
        //{
            //if (discountApplied)
            //{
            //    MessageBox.Show("Rabat został już zastosowany.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}
            //DiscountWindow discountWindow = new DiscountWindow();
            //discountWindow.Owner = this;

            //if (discountWindow.ShowDialog() == true)
            //{
            //    double discountRate = discountWindow.radioButton10.IsChecked == true ? 0.1 : 0.15;

            //    foreach (var orderItem in orderListCollection[currentOrderId])
            //    {
            //        orderItem.Price *= (1 - discountRate);
            //    }
            //    UpdateTotalPrice();
            //    discountApplied = true;
            //}
        //}

        private void AddInvoice_ButtonClick(object sender, RoutedEventArgs e)
        {
            //if (orderListCollection[currentOrderId].Count == 0)
            //{
            //    MessageBox.Show("Brak produktów do dodania do faktury.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //InvoiceWindow invoiceWindow = new InvoiceWindow();
            //if (invoiceWindow.ShowDialog() == true)
            //{
            //    InvoiceCustomerDataDto invoiceCustomerData = InvoiceWindow.InvoiceCustomerDataObject;
            //    MessageBox.Show($"Faktura dla: {invoiceCustomerData.CustomerName}\nAdres: {invoiceCustomerData.CustomerAddress}\nNIP: {invoiceCustomerData.TaxIdentificationNumber}\nSuma: {totalPrice:C2}", "Faktura", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }

        private void ShowFinishedOrders_ButtonClick(object sender, RoutedEventArgs e)
        {
            //var finishedOrders = new FinishedOrdersWindow();
            //finishedOrders.Show();
        }
    }
}
