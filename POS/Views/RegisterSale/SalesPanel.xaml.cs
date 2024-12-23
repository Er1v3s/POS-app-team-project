﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DataAccess.Models;
using POS.Converter;
using POS.Models.Invoices;
using POS.Models.Orders;

namespace POS.Views.RegisterSale
{
    /// <summary>
    /// Logika interakcji dla klasy SalesPanel.xaml
    /// </summary>
    public partial class SalesPanel : Window
    {
        private Employees currentUser;
        public int EmployeeId;
        private double totalPrice = 0;
        private int currentOrderId = 0;
        private bool discountApplied = false;

        ObservableCollection<OrderItemDto> orderList = new ObservableCollection<OrderItemDto>();
        ObservableCollection<ObservableCollection<OrderItemDto>> orderListCollection = new ObservableCollection<ObservableCollection<OrderItemDto>>();

        public SalesPanel(int employeeId)
        {
            orderListCollection.Add(orderList);
            InitializeComponent();

            using (var dbContext = new AppDbContext())
            {
                currentUser = dbContext.Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
            }

            string welcomeMessage = $"{currentUser.FirstName} {currentUser.LastName}";
            SetWelcomeMessage(welcomeMessage);
            LoadAllProducts();
            orderListDataGrid.ItemsSource = orderListCollection[currentOrderId];
            UpdateTotalPrice();
            EmployeeId = employeeId;
        }

        private void MoveToMainWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            Window.GetWindow(this).Close();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderTextBoxHelper.SetPlaceholderOnFocus(sender, e);
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderTextBoxHelper.SetPlaceholderOnLostFocus(sender, e);
        }

        private void SearchTextBox_KeyUp(object sender, TextChangedEventArgs e)
        {
            var searchText = searchTextBox.Text.ToLower();

            if(searchText != null)
            {
                LoadProductsBySearch(searchText);
            }
        }

        private void FilterByCategory_ButtonClick(object sender, RoutedEventArgs e)
        {
            Button categoryButton = sender as Button;
            string category = categoryButton.Content.ToString();
            LoadProductsByCategory(category);
        }

        private void PayForOrder_ButtonClick(object sender, RoutedEventArgs e)
        {
            double totalPrice = Math.Round(orderList.Sum(item => item.Amount * item.Price), 2);

            if (sender is Button button && button.Tag is string paymentMethod)
            {
                OrderSummary summaryOrderWindow = new OrderSummary(orderList);
                summaryOrderWindow.ShowDialog();

                if(summaryOrderWindow.DialogResult == true)
                {
                    var order = SaveOrder();
                    SaveOrderItems(order);
                    SavePayment(order, paymentMethod, totalPrice);
                    RemoveIngredients();
                    orderList.Clear();
                    UpdateTotalPrice();
                    MessageBox.Show($"Zapłacono za zamówienie {totalPrice:C} - metoda płatności: {paymentMethod}");
                } 
            }
        }

        private void RemoveIngredients()
        {
            using (var dbContext = new AppDbContext())
            {
                foreach (var item in orderList)
                {
                    var recipeId = dbContext.Products
                                            .Where(p => p.ProductId == item.Id)
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

        private Orders SaveOrder()
        {
            using (var dbContext = new AppDbContext())
            {
                Orders newOrder = new Orders { OrderTime = DateTime.Now, EmployeeId = EmployeeId };
                var addedOrderEntry = dbContext.Orders.Add(newOrder);
                dbContext.SaveChanges();

                return addedOrderEntry.Entity;
            }
        }

        private void SavePayment(Orders order, string paymentMethod, double totalPrice)
        {
            using (var dbContext = new AppDbContext())
            {
                Payments newPayment = new Payments
                {
                    OrderId = order.OrderId,
                    PaymentMethod = paymentMethod,
                    Amount = totalPrice
                };
                dbContext.Payments.Add(newPayment);
                dbContext.SaveChanges();
            }
        }

        private void SaveOrderItems(Orders order)
        {
            using (var dbContext = new AppDbContext())
            {
                foreach (var orderListItem in orderList)
                {
                    OrderItems newOrderItem = new OrderItems
                    {
                        OrderId = order.OrderId,
                        EmployeeId = EmployeeId,
                        ProductId = orderListItem.Id,
                        Quantity = orderListItem.Amount,
                    };
                    dbContext.OrderItems.Add(newOrderItem);
                    dbContext.SaveChanges();
                }
            }
        }

        private void UpdateTotalPrice()
        {
            totalPrice = orderListCollection[currentOrderId].Sum(item => item.Amount * item.Price);
            totalAmountLabel.Content = $"{totalPrice:C2}";
        }

        private void AddOrUpdateProductInList(Products product)
        {
            var existingProduct = orderListCollection[currentOrderId].FirstOrDefault(p => p.Id == product.ProductId);

            if (existingProduct != null)
            {
                existingProduct.Amount++;
                existingProduct.TotalPrice = Math.Round(existingProduct.Amount * existingProduct.Price, 2);
            }
            else
            {
                orderListCollection[currentOrderId].Add(new OrderItemDto { Id = product.ProductId, Name = product.ProductName, Amount = 1, Price = Convert.ToDouble(product.Price) });
            }
        }

        private void DeleteProductFromOrderList_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (orderListDataGrid.SelectedItem != null)
            {
                var selectedItem = (OrderItemDto)orderListDataGrid.SelectedItem;
                selectedItem.Amount--;
                selectedItem.TotalPrice = Math.Round(selectedItem.Amount * selectedItem.Price, 2);
                if (selectedItem.Amount == 0)
                {
                    orderListCollection[currentOrderId].Remove(selectedItem);
                }
                
                UpdateTotalPrice();
            }
        }

        private Button CreateProductButton(Products product)
        {
            Viewbox viewbox = new Viewbox();
            Button button = new Button
            {
                Style = (Style)FindResource("chooseProductButton"),
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBlock { TextAlignment = TextAlignment.Center, Margin = new Thickness(10), Text = product.ProductName },
                        new TextBlock { TextAlignment = TextAlignment.Center, Margin = new Thickness(10), Text = $"{product.Price} zł" }
                    }
                }
            };

            button.Click += (object sender, RoutedEventArgs e) =>
            {
                AddOrUpdateProductInList(product);
                UpdateTotalPrice();
            };

            ProductsUnifromGrid.Children.Add(viewbox);
            viewbox.Child = button;

            return button;
        }

        private void LoadProducts(IEnumerable<Products> products)
        {
            ProductsUnifromGrid.Children.Clear();
            ProductsUnifromGrid.Columns = 5;

            foreach (var product in products)
            {
                CreateProductButton(product);
            }
        }

        private void LoadAllProducts()
        {
            using (var dbContext = new AppDbContext())
            {
                var products = dbContext.Products.ToList();
                LoadProducts(products);
            }
        }

        private void LoadProductsBySearch(string searchText)
        {
   
            using (var dbContext = new AppDbContext())
            {
                var products = dbContext.Products.Where(p => p.ProductName.ToLower().Contains(searchText)).ToList();
                if(products.Count > 0)
                {
                    LoadProducts(products);
                }
            }
        }

        private void LoadProductsByCategory(string category)
        {
            using (var dbContext = new AppDbContext())
            {
                var products = dbContext.Products.Where(p => p.Category == category).ToList();
                LoadProducts(products);
            }
        }

        private void ShowRecipes_ButtonClick(object sender, RoutedEventArgs e)
        {
            showDrinks_Button.Style = (Style)FindResource("recipeButton");
            showRecipes_Button.Style = (Style)FindResource("payButton");
            ProductsUnifromGrid.Children.Clear();
            ProductsUnifromGrid.Columns = 3;
            if (orderListCollection[currentOrderId].Count == 0)
            {
                TextBlock emptyListTextBlock = new TextBlock
                {
                    Style = (Style)FindResource("RecipeTextStyle"),
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(10),
                    TextWrapping = TextWrapping.Wrap,
                    Text = "Dodaj produkt do zamówienia aby podejrzeć przepis"
                };

                ProductsUnifromGrid.Children.Add(emptyListTextBlock);
            }
            else
            {
                foreach (var orderItem in orderListCollection[currentOrderId])
                {
                    string productName = orderItem.Name;
                    string recipeForProduct = GetRecipe(productName);
                    string recipeIngredients = GetRecipeIngredients(productName);

                    Border textBlockBorder = new Border();
                    textBlockBorder.CornerRadius = new CornerRadius(8);

                    TextBlock textBlock = new TextBlock
                    {
                        Style = (Style)FindResource("RecipeTextStyle"),
                        TextAlignment = TextAlignment.Left,
                        Margin = new Thickness(10),
                        TextWrapping = TextWrapping.Wrap,
                        Text = productName + "\n\n" + recipeIngredients + "\n" + recipeForProduct
                    };

                    ProductsUnifromGrid.Children.Add(textBlock);
                }
            }
        }

        private string GetRecipe(string productName)
        {
            string recipe = "";

            using (var dbContext = new AppDbContext())
            {
                var queryResult = dbContext.Products
                    .Join(dbContext.Recipes, p => p.RecipeId, r => r.RecipeId, (p, r) => new { p, r })
                    .Where(join => join.p.ProductName == productName)
                    .Select(join => join.r.Recipe)
                    .FirstOrDefault();

                if (queryResult != null)
                {
                    recipe = queryResult.ToString();
                }
            }

            return recipe;
        }

        private string GetRecipeIngredients(string productName)
        {
            StringBuilder ingredientsList = new StringBuilder();

            using (var dbContext = new AppDbContext())
            {
                var queryResult = dbContext.Products
                    .Join(dbContext.Recipes, p => p.RecipeId, r => r.RecipeId, (p, r) => new { p, r })
                    .Join(dbContext.RecipeIngredients, j => j.r.RecipeId, ri => ri.RecipeId, (j, ri) => new { j, ri })
                    .Join(dbContext.Ingredients, jri => jri.ri.IngredientId, i => i.IngredientId, (jri, i) => new { jri, i })
                    .Where(join => join.jri.j.p.ProductName == productName)
                    .Select(join => new
                    {
                        IngredientName = join.i.Name,
                        IngredientDescription = join.i.Description,
                        IngredientUnit = join.i.Unit,
                        IngredientQuantity = join.jri.ri.Quantity
                    });

                foreach (var ingredient in queryResult)
                {
                    ingredientsList.AppendLine($"{ingredient.IngredientName} - {ingredient.IngredientQuantity} {ingredient.IngredientUnit}");
                }
            }

            return ingredientsList.ToString();
        }

        private void ShowDrinks_ButtonClick(object sender, RoutedEventArgs e)
        {
            showDrinks_Button.Style = (Style)FindResource("payButton");
            showRecipes_Button.Style = (Style)FindResource("recipeButton");
            ProductsUnifromGrid.Children.Clear();
            LoadAllProducts();
        }

        private void ShowOpenOrders_ButtonClick(object sender, RoutedEventArgs e)
        {
            ProductsUnifromGrid.Children.Clear();
            ProductsUnifromGrid.Columns = 5;

            CreateNewOrderButton();

            for (int i = 0; i < orderListCollection.Count; i++)
            {
                int orderId = i;
                double totalPriceForOrder = CalculateTotalPriceForOrder(orderListCollection[orderId]);

                CreateOrderButton(orderId, totalPriceForOrder);
            }
        }

        private double CalculateTotalPriceForOrder(ObservableCollection<OrderItemDto> order)
        {
            return order.Sum(item => item.Amount * item.Price);
        }

        private void DeleteCurrentOrder_ButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Spowoduje to utratę aktualnie wyświetlonego zamówienia.\n Czy chcesz kontynuować?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (orderListCollection.Count > 1)
                {
                    removeOrder();
                }
                else
                {
                    ObservableCollection<OrderItemDto> newOrder = new ObservableCollection<OrderItemDto>();
                    orderListCollection.Add(newOrder);
                    removeOrder();
                }
            }
        }

        private Button CreateNewOrderButton()
        {
            Viewbox viewbox = new Viewbox();
            Button newOrderButton = new Button
            {
                Style = (Style)FindResource("createNewOrderButton"),
                Content = "Nowe zamówienie"
            };

            newOrderButton.Click += (object sender, RoutedEventArgs e) =>
            {
                ObservableCollection<OrderItemDto> newOrder = new ObservableCollection<OrderItemDto>();
                orderListCollection.Add(newOrder);
                currentOrderId = orderListCollection.Count - 1;
                orderListDataGrid.ItemsSource = orderListCollection[currentOrderId];
                UpdateTotalPrice();
                ShowRecipes_ButtonClick(null, null);
                LoadAllProducts();
            };

            ProductsUnifromGrid.Children.Add(viewbox);
            viewbox.Child = newOrderButton;
            return newOrderButton;
        }

        private Button CreateOrderButton(int orderId, double totalPriceForOrder)
        {
            Viewbox viewbox = new Viewbox();
            Button orderButton = new Button
            {
                Style = (Style)FindResource("chooseProductButton"),
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBlock { TextAlignment = TextAlignment.Center, Margin = new Thickness(10), Text = $"Zamówienie {orderId + 1}" },
                        new TextBlock { TextAlignment = TextAlignment.Center, Margin = new Thickness(10), Text = $"Suma: {totalPriceForOrder:C2}" }
                    }
                }
            };

            orderButton.Click += (object sender, RoutedEventArgs e) =>
            {
                currentOrderId = orderId;
                orderListDataGrid.ItemsSource = orderListCollection[currentOrderId];
                UpdateTotalPrice();
                ShowRecipes_ButtonClick(null, null);
                LoadAllProducts();
            };

            ProductsUnifromGrid.Children.Add(viewbox);
            viewbox.Child = orderButton;
            return orderButton;
        }

        private void removeOrder()
        {
            orderListCollection.RemoveAt(currentOrderId);
            currentOrderId = 0;
            orderListDataGrid.ItemsSource = orderListCollection[currentOrderId];
            UpdateTotalPrice();
            LoadAllProducts();

            discountApplied = false;
        }

        private void SetWelcomeMessage(string message)
        {
            welcomeLabel.Content = message;
        }
        
        private void ApplyDiscount_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (discountApplied)
            {
                MessageBox.Show("Rabat został już zastosowany.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            DiscountWindow discountWindow = new DiscountWindow();
            discountWindow.Owner = this;

            if (discountWindow.ShowDialog() == true)
            {
                double discountRate = discountWindow.radioButton10.IsChecked == true ? 0.1 : 0.15;

                foreach (var orderItem in orderListCollection[currentOrderId])
                {
                    orderItem.Price *= (1 - discountRate);
                }
                UpdateTotalPrice();
                discountApplied = true;
            }
        }

        private void AddInvoice_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (orderListCollection[currentOrderId].Count == 0)
            {
                MessageBox.Show("Brak produktów do dodania do faktury.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            InvoiceWindow invoiceWindow = new InvoiceWindow();
            if (invoiceWindow.ShowDialog() == true)
            {
                InvoiceCustomerDataDto invoiceCustomerData = InvoiceWindow.InvoiceCustomerDataObject;
                MessageBox.Show($"Faktura dla: {invoiceCustomerData.CustomerName}\nAdres: {invoiceCustomerData.CustomerAddress}\nNIP: {invoiceCustomerData.TaxIdentificationNumber}\nSuma: {totalPrice:C2}", "Faktura", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ShowFinishedOrders_ButtonClick(object sender, RoutedEventArgs e)
        {
            FinishedOrders finishedOrders = new FinishedOrders();
            finishedOrders.Show();
        }
    }
}
