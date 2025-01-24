namespace POS.Models.Invoices
{
    public class InvoiceDto
    {
        public required string TaxIdentificationNumber { get; set; }
        public required string CustomerName { get; set; }
        public required string CustomerAddress { get; set; }
    }
}
