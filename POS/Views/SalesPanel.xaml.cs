using POS.Data;
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
    public partial class SalesPanel : Window
    {
        public SalesPanel()
        {
            InitializeComponent();
            LoadProducts();

            searchTextBox.GotFocus += SearchTextBox_GotFocus;
            searchTextBox.LostFocus += SearchTextBox_LostFocus;

            ObservableCollection<OrderItem> orderList = new ObservableCollection<OrderItem>();


            orderList.Add(new OrderItem { id = 1, name="Cuba Libre", amount=2, price=22.99 }); 
            orderList.Add(new OrderItem { id = 1, name="Wodka & Cola", amount=1, price=12.99 }); 
            orderList.Add(new OrderItem { id = 1, name="Margharita", amount=1, price=31.99 }); 
            orderList.Add(new OrderItem { id = 1, name="Wodka Stock 0,7", amount=1, price=69.99 });

            orderListDataGrid.ItemsSource = orderList;
        }

        private void Move_To_Main_Window(object sender, RoutedEventArgs e)
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
                                new TextBlock { TextAlignment = TextAlignment.Center, Margin = new Thickness(10), Text = product.Name },
                                new TextBlock { TextAlignment = TextAlignment.Center, Margin = new Thickness(10), Text = $"{product.Price} zł" }
                            }
                        }
                    };

                    // button onClick:
                    button.Click += (sender, args) =>
                    {
                        //Method
                    };

                    ProductsWrapPanel.Children.Add(button);
                }
            }
        }
    }

    public class OrderItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public int amount { get; set; }
        public double price { get; set; }
    }
}
