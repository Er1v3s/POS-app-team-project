using POS.Models.Invoices;

namespace POS.Services.SalesPanel
{
    public class InvoiceService
    {
        private InvoiceDto? invoiceData;

        public InvoiceDto? GetInvoiceCustomerData()
        {
            return invoiceData;
        }

        public bool ValidateAndSetInvoice(InvoiceDto invoiceDto)
        {
            var result = ValidateInvoiceData(invoiceDto);

            if (result)
                invoiceData = invoiceDto;

            return result;
        }

        private bool ValidateInvoiceData(InvoiceDto invoiceCustomerDto)
        {
            return invoiceCustomerDto.TaxIdentificationNumber.Length == 10
                   && invoiceCustomerDto.CustomerName.Length > 0
                   && invoiceCustomerDto.CustomerAddress.Length > 0;
        }
    }
}