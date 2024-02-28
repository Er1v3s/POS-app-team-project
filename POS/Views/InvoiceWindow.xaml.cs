using System.Windows;
using System.Windows.Input;

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

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void CloseWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveInvoice_ButtonClick(object sender, RoutedEventArgs e)
        {
            // TODO
            // Dodawanie do zamówienia danych do faktury


            //ClientName = clientNameTextBox.Text;
            //DeliveryAddress = deliveryAddressTextBox.Text;

            this.DialogResult = true;
            this.Close();
        }
    }
}
