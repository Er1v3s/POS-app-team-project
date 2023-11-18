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
        public SalesPanel()
        {
            InitializeComponent();

            ObservableCollection<OrderItem> orderList = new ObservableCollection<OrderItem>();

            orderList.Add(new OrderItem { Id = 1, Name = "Wodka & Cola", Amount = 1, Price = 12.99 });
            orderList.Add(new OrderItem { Id = 1, Name = "Margharita", Amount = 1, Price = 31.99 });
            orderList.Add(new OrderItem { Id = 1, Name = "Wodka Stock 0,7", Amount = 1, Price = 69.99 });

            orderListDataGrid.ItemsSource = orderList;
        }


        

        private void MoveToMainWindow(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            
            Window.GetWindow(this).Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e, ref ObservableCollection<OrderItem> orderList)
        {
            
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
