﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Products
    {
        public required int Product_id { get; set; }
        public required string Product_name { get; set; }
        public required string Category { get; set; }
        public string? Description { get; set; }
        public required double? Price { get; set; }
        public Ingredients? Ingredients { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
