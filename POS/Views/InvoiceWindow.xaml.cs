using System.Windows;

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
