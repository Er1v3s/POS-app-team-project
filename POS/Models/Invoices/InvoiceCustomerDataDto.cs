namespace POS.Models.Invoices
{
    public class InvoiceCustomerDataDto
    {
        public required string TaxIdentificationNumber { get; set; }
        public required string CustomerName { get; set; }
        public required string CustomerAddress { get; set; }
    }
}
