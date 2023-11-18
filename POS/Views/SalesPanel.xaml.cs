using POS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private double totalAmount = 0;
        ObservableCollection<OrderItem> orderList = new ObservableCollection<OrderItem>();

        public SalesPanel()
        {
            InitializeComponent();
            LoadProducts();

            searchTextBox.GotFocus += SearchTextBox_GotFocus;
            searchTextBox.LostFocus += SearchTextBox_LostFocus;

            orderList.Add(new OrderItem { Id = 1, Name = "Wodka & Cola", Amount = 1, Price = 12.99 });
            orderList.Add(new OrderItem { Id = 1, Name = "Margharita", Amount = 1, Price = 31.99 });
            orderList.Add(new OrderItem { Id = 1, Name = "Wodka Stock 0,7", Amount = 1, Price = 69.99 });
            orderList.Add(new OrderItem { Id = 1, Name = "Cuba Libre", Amount = 2, Price = 22.99 });
            orderList.Add(new OrderItem { Id = 1, Name = "Wodka & Cola", Amount = 1, Price = 12.99 });
            orderList.Add(new OrderItem { Id = 1, Name = "Margharita", Amount = 5, Price = 31.99 });
            orderList.Add(new OrderItem { Id = 1, Name = "Wodka Stock 0,7", Amount = 1, Price = 69.99 });

            orderListDataGrid.ItemsSource = orderList;

            UpdateTotalAmount();
        }

        private void UpdateTotalAmount()
        {
            totalAmount = 0;

            foreach (OrderItem item in orderListDataGrid.ItemsSource)
            {
                totalAmount += item.Amount * item.Price;
            }
            totalAmountLabel.Content = $"{totalAmount:C2}";
        }

        // onClick menuitem button method
       /* private void Button_Click(object sender, RoutedEventArgs e)
        {
            orderList.Add(new OrderItem { Id = 1, Name = "Cuba Libre", Amount = 1, Price = 18.99 });
        } */
        private void MoveToMainWindow(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            Window.GetWindow(this).Close();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text == "Szukaj")
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
                        orderList.Add(new OrderItem { Id = 1, Name = product.Product_name, Amount = 1, Price = Convert.ToDouble(product.Price) });
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
                            orderList.Add(new OrderItem { Id = 1, Name = product.Product_name, Amount = 1, Price = Convert.ToDouble(product.Price) });
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
                        orderList.Add(new OrderItem { Id = 1, Name = product.Product_name, Amount = 1, Price = Convert.ToDouble(product.Price) });
                    };

                    ProductsWrapPanel.Children.Add(button);
                }
            }
        }

    }

    public class OrderItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
    }
}
