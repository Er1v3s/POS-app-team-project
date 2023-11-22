using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class RecipeIngredients
    {
        public required int RecipeIngredient_id {  get; set; }
        public required int Recipe_id { get; set; }
        public required int Ingredient_id { get; set; }
        public required double Quantity { get; set; }
    }
}
