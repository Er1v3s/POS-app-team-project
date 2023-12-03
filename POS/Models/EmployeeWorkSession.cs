using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class EmployeeWorkSession
    {
        public int Work_Session_Id { get; set; }
        public int? Employee_Id { get; set; }
        public string? Employee_Name { get; set; }
        public string? Working_Time_From { get; set; }
        public string? Working_Time_To { get; set; }
        public string? Working_Time_Summary { get; set; }
    }
}
