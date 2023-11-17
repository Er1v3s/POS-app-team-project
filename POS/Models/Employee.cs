using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public required string first_name { get; set; }
        public required string last_name { get; set; }
        public string? email { get; set; }
        public int? phone_number { get; set; }
        public string? address { get; set; }
        public required DateTime hire_date { get; set; }
        public string? job_title { get; set; }
        public required string login { get; set; }
        public required string password { get; set; }



    }
}
