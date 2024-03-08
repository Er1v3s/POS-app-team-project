using POS.ViewModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy InvoiceWindow.xaml
    /// </summary>
    public partial class InvoiceWindow : Window
    {
        public static InvoiceCustomerData InvoiceCustomerDataObject;

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
            try
            {
                InvoiceCustomerData invoiceCustomerData = CreateInvoiceCustomerDataObject();
                ValidateInvoiceCustomerData(invoiceCustomerData);

                InvoiceCustomerDataObject = invoiceCustomerData;

                this.DialogResult = true;
                this.Close();
            } 
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void TaxIdentificationNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.BorderThickness = new Thickness(2);

            if (textBox.Text.Length != 10) 
            {
                await Task.Delay(1500);

                if (textBox.Text.Length != 10)
                {
                    taxIdentificationNumberWarning.Text = "Niepoprawna długość numeru NIP";
                    textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(174, 75, 89));
                }
            }
            else
            {
                taxIdentificationNumberWarning.Text = "";
                textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(55, 154, 140));
            }
        }

        private int ParseTaxIdentificationNumber(string taxIdentificationNumber)
        {
            int parsedTaxIdentificationNumber;

            if (int.TryParse(taxIdentificationNumber, out parsedTaxIdentificationNumber))
            {
                return parsedTaxIdentificationNumber;
            }
            else
            {
                MessageBox.Show("NIP nieprawidłowy");
                txtTaxIdentificationNumber.Text = "";
                return 0;
            }
        }

        private InvoiceCustomerData CreateInvoiceCustomerDataObject()
        {
            return new InvoiceCustomerData()
            {
                TaxIdentificationNumber = ParseTaxIdentificationNumber(txtTaxIdentificationNumber.Text),
                CustomerName = txtCustomerName.Text,
                CustomerAddress = txtCustomerAddress.Text,
            };
        }

        private void ValidateInvoiceCustomerData(InvoiceCustomerData invoiceCustomerData)
        {
            if(invoiceCustomerData.TaxIdentificationNumber.ToString().Length != 10
                || invoiceCustomerData.CustomerName.ToString().Length < 1
                || invoiceCustomerData.CustomerAddress.ToString().Length < 1
                )
            {
                throw new Exception("Wprowadzone dane są nieprawidłowe");
            }
        }
    }
}
