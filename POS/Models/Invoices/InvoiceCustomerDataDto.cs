using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.Invoices
{
    public class InvoiceCustomerDataDto
    {
        public int TaxIdentificationNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
    }
}
