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

        public ICommand SetInvoiceDataCommand { get; }

        public InvoiceViewModel(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;

            SetInvoiceDataCommand = new RelayCommand(SetInvoiceData);
        }

        private void SetInvoiceData()
        {
            var invoiceDto = CreateInvoiceDto();
            var result = _invoiceService.ValidateAndSetInvoice(invoiceDto);

            if (result)
                DialogResult = true;
            else
                MessageBox.Show("Walidacja nie przebiegła poprawnie");
        }

        private InvoiceDto CreateInvoiceDto()
        {
            return new InvoiceDto()
            {
                TaxIdentificationNumber = taxIdentificationNumber,
                CustomerName = customerName,
                CustomerAddress = customerAddress,
            };
        }
    }
}