using System;
using System.Collections.Generic;
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
    /// Logika interakcji dla klasy InvoiceWindow.xaml
    /// </summary>
    public partial class InvoiceWindow : Window
    {
        public string ClientName { get; private set; }
        public string DeliveryAddress { get; private set; }

        public InvoiceWindow()
        {
            InitializeComponent();
        }

        private void SaveInvoice_ButtonClick(object sender, RoutedEventArgs e)
        {
            ClientName = clientNameTextBox.Text;
            DeliveryAddress = deliveryAddressTextBox.Text;

            this.DialogResult = true;
            this.Close();
        }
    }
}
