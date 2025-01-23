using POS.Models.Invoices;
using POS.Views.Windows.SalesPanel;

namespace POS.Services.SalesPanel
{
    public class InvoiceService
    {
        private InvoiceCustomerDataDto? _invoiceCustomerData;

        public InvoiceCustomerDataDto? GetInvoiceCustomerData()
        {
            return _invoiceCustomerData;
        }

        public bool ValidateAndSaveInvoice(InvoiceCustomerDataDto invoiceCustomerData)
        {
            var result = ValidateInvoiceCustomerData(invoiceCustomerData);

            if (result)
                _invoiceCustomerData = invoiceCustomerData;

            return result;
        }

        private bool ValidateInvoiceCustomerData(InvoiceCustomerDataDto invoiceCustomerData)
        {
            return invoiceCustomerData.TaxIdentificationNumber.Length == 10
                   && invoiceCustomerData.CustomerName.Length > 0
                   && invoiceCustomerData.CustomerAddress.Length > 0;
        }
    }
}
