﻿using System.Collections.Generic;

namespace POS.Models
{
    public class Recipes
    {
        public int RecipeId { get; set; }
        public required string RecipeName { get; set; }
        public required string Recipe {  get; set; }
        public ICollection<RecipeIngredients> RecipeIngredients { get; set; }
    }
}
