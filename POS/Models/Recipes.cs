using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Recipes
    {
        public int Recipe_id { get; set; }
        public required string Recipe_name { get; set; }
        public required string Recipe {  get; set; }
        public ICollection<RecipeIngredients> RecipeIngredients { get; set; }
    }
}
