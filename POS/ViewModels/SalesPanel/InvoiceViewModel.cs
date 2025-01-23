using System.Windows;
using System.Windows.Input;
using POS.Models.Invoices;
using POS.Services.SalesPanel;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.SalesPanel
{
    public class InvoiceViewModel : ViewModelBase
    {
        private readonly InvoiceService _invoiceService;

        private bool dialogResult;

        private string taxIdentificationNumber;
        private string customerName;
        private string customerAddress;

        public bool DialogResult
        {
            get => dialogResult;
            set => SetField(ref dialogResult, value);
        }

        public string TaxIdentificationNumber
        {
            get => taxIdentificationNumber;
            set => SetField(ref taxIdentificationNumber, value);
        }

        public string CustomerName
        {
            get => customerName;
            set => SetField(ref customerName, value);
        }

        public string CustomerAddress
        {
            get => customerAddress;
            set => SetField(ref customerAddress, value);
        }

        public ICommand SaveInvoiceCommand { get; }

        public InvoiceViewModel(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;

            SaveInvoiceCommand = new RelayCommand(SaveInvoice);
        }

        private void SaveInvoice()
        {
            var invoiceCustomerData = CreateInvoiceCustomerDataObject();
            var result = _invoiceService.ValidateAndSaveInvoice(invoiceCustomerData);

            if (result)
                DialogResult = true;
            else
                MessageBox.Show("Walidacja nie przebiegła poprawnie");
        }

        private InvoiceCustomerDataDto CreateInvoiceCustomerDataObject()
        {
            return new InvoiceCustomerDataDto()
            {
                TaxIdentificationNumber = taxIdentificationNumber,
                CustomerName = customerName,
                CustomerAddress = customerAddress,
            };
        }
    }
}