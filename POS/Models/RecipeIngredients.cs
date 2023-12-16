using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class RecipeIngredients
    {
        [Key]public int RecipeIngredient_id {  get; set; }
        public int Recipe_id { get; set; }
        public Recipes Recipe { get; set; }
        public int Ingredient_id { get; set; }
        public Ingredients Ingredient { get; set; }
        public required double Quantity { get; set; }
    }
}
