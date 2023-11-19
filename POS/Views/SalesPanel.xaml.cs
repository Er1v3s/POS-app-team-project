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
        private double totalPrice = 0;
        ObservableCollection<OrderItem> orderList = new ObservableCollection<OrderItem>();

        public SalesPanel()
        {
            InitializeComponent();
            LoadProducts();
            orderListDataGrid.ItemsSource = orderList;
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

        private void UpdateTotalPrice()
        {
            totalPrice = orderList.Sum(item => item.Amount * item.Price);
            totalAmountLabel.Content = $"{totalPrice:C2}";
        }


        private void AddOrUpdateProductInList(Products product)
        {
            var existingProduct = orderList.FirstOrDefault(p => p.Id == product.Product_id);

            if (existingProduct != null)
            {
                existingProduct.Amount++;
            }
            else
            {
                orderList.Add(new OrderItem { Id = product.Product_id, Name = product.Product_name, Amount = 1, Price = Convert.ToDouble(product.Price) });
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
                    orderList.Remove(selectedItem);
                }

                UpdateTotalPrice();
            }
        }
        private void LoadProducts()
        {
            using (var dbContext = new AppDbContext())
            {
                var products = dbContext.Products.ToList();

                foreach (var product in products)
                {
                    Button button = new Button
                    {
                        Style = (Style)FindResource("chooseProductButton"), // Styl przycisku zasobu z XAML
                        Content = new StackPanel
                        {
                            Children =
                            {
                                new TextBlock { TextAlignment = TextAlignment.Center, Margin = new Thickness(10), Text = product.Product_name },
                                new TextBlock { TextAlignment = TextAlignment.Center, Margin = new Thickness(10), Text = $"{product.Price} zł" }
                            }
                        }
                    };

                    // button onClick:
                    button.Click += (object sender, RoutedEventArgs e) =>
                    {
                        AddOrUpdateProductInList(product);
                        UpdateTotalPrice();
                    };

                    ProductsWrapPanel.Children.Add(button);
                }
            }
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var searchText = searchTextBox.Text.ToLower();

                using (var dbContext = new AppDbContext())
                {
                    var products = dbContext.Products.Where(p => p.Product_name.ToLower().Contains(searchText)).ToList();

                    ProductsWrapPanel.Children.Clear();

                    foreach (var product in products)
                    {
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

                        button.Click += (object buttonSender, RoutedEventArgs buttonE) =>
                        {
                            AddOrUpdateProductInList(product);
                            UpdateTotalPrice();
                        };

                        ProductsWrapPanel.Children.Add(button);
                    }
                }

                e.Handled = true;
                searchTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            Button categoryButton = sender as Button;
            string category = categoryButton.Content.ToString();

            using (var dbContext = new AppDbContext())
            {
                var products = dbContext.Products.Where(p => p.Category == category).ToList();

                // Wyczyść aktualnie wyświetlone produkty
                ProductsWrapPanel.Children.Clear();

                foreach (var product in products)
                {
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

                    // button onClick:
                    button.Click += (object buttonSender, RoutedEventArgs buttonE) =>
                    {
                        AddOrUpdateProductInList(product);
                        UpdateTotalPrice();
                    };

                    ProductsWrapPanel.Children.Add(button);
                }
            }
        }
    }
}
