using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Product
    {
        [Key]
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Category { get; set; }
        public required string Description { get; set; }
        public required string Price { get; set; }
        public Ingredients? Ingredients { get; set; }
        public required bool Available { get; set; }
    }
}
