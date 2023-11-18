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
        private double totalAmount = 0;
        public SalesPanel()
        {
            InitializeComponent();

            ObservableCollection<OrderItem> orderList = new ObservableCollection<OrderItem>();


            orderList.Add(new OrderItem { id = 1, name="Cuba Libre", amount=2, price=22.99 }); 
            orderList.Add(new OrderItem { id = 1, name="Wodka & Cola", amount=1, price=12.99 }); 
            orderList.Add(new OrderItem { id = 1, name="Margharita", amount=5, price=31.99 }); 
            orderList.Add(new OrderItem { id = 1, name="Wodka Stock 0,7", amount=1, price=69.99 });

            orderListDataGrid.ItemsSource = orderList;

            UpdateTotalAmount();
        }

        private void UpdateTotalAmount()
        {
            totalAmount = 0;

            foreach (OrderItem item in orderListDataGrid.ItemsSource)
            {
                totalAmount += item.amount * item.price;
            }
            totalAmountLabel.Content = $"{totalAmount:C2}";
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
