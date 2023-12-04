using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.ViewModel
{
    public class OrderHistory
    {
        public int Order_Id { get; set; }
        public string Employee_Name {  get; set; }
        public string Order_Date { get; set; }
        public string Order_Time { get; set; }

    }
}
