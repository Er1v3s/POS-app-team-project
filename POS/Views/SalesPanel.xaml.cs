using Microsoft.EntityFrameworkCore;
using POS.Models;
using POS.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy SalesPanel.xaml
    /// </summary>
    public partial class SalesPanel
    {
        private Employees currentUser;
        private double totalPrice = 0;
        private int currentOrderId = 0;
        ObservableCollection<OrderItem> orderList = new ObservableCollection<OrderItem>();
        ObservableCollection<ObservableCollection<OrderItem>> orderListCollection = new ObservableCollection<ObservableCollection<OrderItem>>();
        public SalesPanel(int employeeId)
        {
            orderListCollection.Add(orderList);
            InitializeComponent();
            using (var dbContext = new AppDbContext())
            {
                currentUser = dbContext.Employees.FirstOrDefault(e => e.Employee_id == employeeId);
            }
            string welcomeMessage = $"{currentUser.First_name} {currentUser.Last_name}";
            SetWelcomeMessage(welcomeMessage);
            LoadAllProducts();
            orderListDataGrid.ItemsSource = orderListCollection[currentOrderId];
            UpdateTotalPrice();
        }

        private void MoveToMainWindow(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            Window.GetWindow(this).Close();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text.Length > 0)
            {
                searchTextBox.Text = "";
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                searchTextBox.Text = "Szukaj";
            }
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var searchText = searchTextBox.Text.ToLower();
                LoadProductsBySearch(searchText);

                e.Handled = true;
                searchTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            Button categoryButton = sender as Button;
            string category = categoryButton.Content.ToString();
            LoadProductsByCategory(category);
        }

        private void UpdateTotalPrice()
        {
            totalPrice = orderListCollection[currentOrderId].Sum(item => item.Amount * item.Price);
            totalAmountLabel.Content = $"{totalPrice:C2}";
        }

        private void AddOrUpdateProductInList(Products product)
        {
            var existingProduct = orderListCollection[currentOrderId].FirstOrDefault(p => p.Id == product.Product_id);

            if (existingProduct != null)
            {
                existingProduct.Amount++;
            }
            else
            {
                orderListCollection[currentOrderId].Add(new OrderItem { Id = product.Product_id, Name = product.Product_name, Amount = 1, Price = Convert.ToDouble(product.Price) });
            }
        }

        private void DeleteProductFromOrderList(object sender, RoutedEventArgs e)
        {
            if (orderListDataGrid.SelectedItem != null)
            {
                var selectedItem = (OrderItem)orderListDataGrid.SelectedItem;
                selectedItem.Amount--;

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
                        new TextBlock { TextAlignment = TextAlignment.Center, Margin = new Thickness(10), Text = product.Product_name },
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
                var products = dbContext.Products.Where(p => p.Product_name.ToLower().Contains(searchText)).ToList();
                LoadProducts(products);
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

        private void ShowRecipes(object sender, RoutedEventArgs e)
        {
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
                    .Join(dbContext.Recipes, p => p.Recipe_id, r => r.Recipe_id, (p, r) => new { p, r })
                    .Where(join => join.p.Product_name == productName)
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
                    .Join(dbContext.Recipes, p => p.Recipe_id, r => r.Recipe_id, (p, r) => new { p, r })
                    .Join(dbContext.RecipeIngredients, j => j.r.Recipe_id, ri => ri.Recipe_id, (j, ri) => new { j, ri })
                    .Join(dbContext.Ingredients, jri => jri.ri.Ingredient_id, i => i.Ingredient_id, (jri, i) => new { jri, i })
                    .Where(join => join.jri.j.p.Product_name == productName)
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

        private void ShowDrinks(object sender, RoutedEventArgs e)
        {
            ProductsUnifromGrid.Children.Clear();
            LoadAllProducts();
        }

        private void ShowOpenOrders(object sender, RoutedEventArgs e)
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

        private double CalculateTotalPriceForOrder(ObservableCollection<OrderItem> order)
        {
            return order.Sum(item => item.Amount * item.Price);
        }

        private void DeleteCurrentOrder(object sender, RoutedEventArgs e)
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
                    ObservableCollection<OrderItem> newOrder = new ObservableCollection<OrderItem>();
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
                ObservableCollection<OrderItem> newOrder = new ObservableCollection<OrderItem>();
                orderListCollection.Add(newOrder);
                currentOrderId = orderListCollection.Count - 1; // Ustawienie bieżącego indeksu zamówienia na ostatnie dodane
                orderListDataGrid.ItemsSource = orderListCollection[currentOrderId]; // Ustawienie nowej kolekcji zamówień jako źródła danych dla orderListDataGrid
                UpdateTotalPrice();
                ShowRecipes(null, null);
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
                ShowRecipes(null, null);
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
        private bool discountApplied = false;
        private void ApplyDiscount_Click(object sender, RoutedEventArgs e)
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

        private void InvoiceAdd_Click(object sender, RoutedEventArgs e)
        {
            if (orderListCollection[currentOrderId].Count == 0)
            {
                MessageBox.Show("Brak produktów do dodania do faktury.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            InvoiceWindow invoiceWindow = new InvoiceWindow();
            if (invoiceWindow.ShowDialog() == true)
            {
                string clientName = invoiceWindow.ClientName;
                string deliveryAddress = invoiceWindow.DeliveryAddress;

                MessageBox.Show($"Faktura dla: {clientName}\nAdres dostawy: {deliveryAddress}\nSuma: {totalPrice:C2}", "Faktura", MessageBoxButton.OK, MessageBoxImage.Information);

                orderListCollection[currentOrderId].Clear();
                UpdateTotalPrice();
                orderListDataGrid.Items.Refresh();
            }
        }

    }
}
