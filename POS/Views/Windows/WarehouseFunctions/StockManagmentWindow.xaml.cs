using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using POS.Helpers;
using POS.Services;
using POS.ViewModels.WarehouseFunctions;

namespace POS.Views.Windows.WarehouseFunctions
{
    /// <summary>
    /// Logika interakcji dla klasy StockManagment.xaml
    /// </summary>
    public partial class StockManagementWindow : Window
    {
        public StockManagementWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<StockManagementViewModel>();

            var viewModel = (StockManagementViewModel)DataContext;
            viewModel.CloseWindowBaseAction = Close;

            //FillEditIngredientComboBox(EditIngredient_ComboBox);
            //FillEditProductComboBox(EditProduct_ComboBox);
            //FillEditIngredientComboBox(EditRecipeIngredient_ComboBox);
            //FillEditProductComboBox(EditRecipeOfProduct_ComboBox);
        }

        #region left side

        //private void EditRecipeOfProduct_ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
            //if (EditRecipeOfProduct_ComboBox.SelectedItem != null)
            //{
            //    string selectedProduct = EditRecipeOfProduct_ComboBox.SelectedItem.ToString();

            //    LoadIngredientsToDataGrid(selectedProduct);
            //}
        //}

        //private void LoadIngredientsToDataGrid(string SelectedProduct)
        //{
            //using (var dbContext = new AppDbContext())
            //{
            //    var product = dbContext.Product.FirstOrDefault(i => i.ProductName == SelectedProduct);

            //    if (product != null)
            //    {
            //        var ingredients = dbContext.RecipeIngredients
            //            .Where(ri => ri.RecipeId == product.RecipeId)
            //            .Join(dbContext.Ingredients,
            //                ri => ri.IngredientId,
            //                i => i.IngredientId,
            //                (ri, i) => new
            //                {
            //                    RecipeIngredient_id = ri.RecipeIngredientId,
            //                    Ingredient_id = ri.IngredientId,
            //                    ri.Quantity,
            //                    i.Name,
            //                    i.Unit
            //                })
            //            .ToList();

            //        recipeIngredientsDataGrid.ItemsSource = ingredients;
            //        EditRecipeIngredient_ComboBox.IsEnabled = true;
            //    }
            //}
        //}
        
        //private void EditRecipeIngredient_ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
            //if(EditRecipeIngredient_ComboBox != null)
            //{
            //    string selectedIngredient = EditRecipeIngredient_ComboBox.SelectedItem.ToString();

            //    using (var dbContext = new AppDbContext())
            //    {
            //        var recipeIngredient = dbContext.Ingredients.FirstOrDefault(i => i.Name == selectedIngredient);
            //        if(recipeIngredient != null)
            //        {
            //            RecipeIngredientQuantity.IsEnabled = true;
            //        }
            //    }
            //}
        //}

        private void DeleteRecipeIngredient_ButtonClick(object sender, RoutedEventArgs e)
        {
            //int IngredientId = 0, RecipeId = 0;
            //string selectedIngredient = EditRecipeIngredient_ComboBox.SelectedItem?.ToString();
            //string selectedProduct = EditRecipeOfProduct_ComboBox.SelectedItem?.ToString();

            //if (string.IsNullOrEmpty(selectedIngredient) || string.IsNullOrEmpty(selectedProduct))
            //{
            //    MessageBox.Show("Wybierz składnik i produkt przed usunięciem.");
            //    return;
            //}

            //using (var dbContext = new AppDbContext())
            //{
            //    var ingredient = dbContext.Ingredients.FirstOrDefault(i => i.Name == selectedIngredient);
            //    if (ingredient != null)
            //    {
            //        IngredientId = ingredient.IngredientId;
            //    }

            //    var product = dbContext.Product.FirstOrDefault(p => p.ProductName == selectedProduct);
            //    if (product != null)
            //    {
            //        RecipeId = product.RecipeId;
            //    }

            //    try
            //    {
            //        dbContext.Database.ExecuteSqlRaw("DELETE FROM RecipeIngredients WHERE Ingredient_id = {0} AND Recipe_id = {1}", IngredientId, RecipeId);
            //        dbContext.SaveChanges();
            //        MessageBox.Show("Rekord usunięty pomyślnie.");
            //        LoadIngredientsToDataGrid(selectedProduct);
            //        EditRecipeIngredient_ComboBox.SelectedIndex = 0;
            //        RecipeIngredientQuantity.Text = "Ilość składnika w przepisie";
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"Wystąpił błąd: {ex.Message}");
            //    }
            //}
        }

        private void AddRecipeIngredient_ButtonClick(object sender, RoutedEventArgs e)
        {
            //int IngredientId = 0, RecipeId = 0;
            //string selectedIngredient = EditRecipeIngredient_ComboBox.SelectedItem.ToString();
            //string selectedProduct = EditRecipeOfProduct_ComboBox.SelectedItem.ToString();
            //int Quantity;
            //if (int.TryParse(RecipeIngredientQuantity.Text, out Quantity))
            //{
            //    Quantity = Convert.ToInt32(RecipeIngredientQuantity.Text);

            //    using (var dbContext = new AppDbContext())
            //    {
            //        var product = dbContext.Product.FirstOrDefault(p => p.ProductName == selectedProduct);
            //        if (product != null)
            //        {
            //            RecipeId = product.RecipeId;
            //        }

            //        var ingredient = dbContext.Ingredients.FirstOrDefault(i => i.Name == selectedIngredient);
            //        if (ingredient != null)
            //        {
            //            IngredientId = ingredient.IngredientId;
            //        }

            //        var recipe = dbContext.Recipes.FirstOrDefault(r => r.RecipeId == RecipeId);
            //        if (ingredient != null && recipe != null)
            //        {

            //            var recipeIngredient = new RecipeIngredient
            //            {
            //                RecipeId = RecipeId,
            //                IngredientId = IngredientId,
            //                Quantity = Quantity
            //            };

            //            dbContext.Database.ExecuteSqlRaw("INSERT INTO RecipeIngredients (Recipe_id, Ingredient_id, Quantity) VALUES ({0}, {1}, {2})", RecipeId, ingredient.IngredientId, Quantity);
            //            dbContext.SaveChanges();

            //            MessageBox.Show("Rekord dodany pomyślnie.");
            //        }
            //        LoadIngredientsToDataGrid(selectedProduct);
            //        EditRecipeIngredient_ComboBox.SelectedIndex = 0;
            //        RecipeIngredientQuantity.Text = "Ilość składnika w przepisie";
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Wprowadzona ilość nie jest cyfrą");
            //}
        }

        #endregion

        #region middle side

        private void FillEditProductComboBox(ComboBox ComboBoxName)
        {
            //using (var dbContext = new AppDbContext())
            //{
            //    var products = dbContext.Product.Select(i => i.ProductName).ToList();

            //    foreach (var product in products)
            //    {
            //        ComboBoxName.Items.Add(product);
            //    }
            //}
        }

        private void EditProduct_ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (EditProduct_ComboBox.SelectedItem != null)
            //{
            //    int recipeId = 0;
            //    string selectedProduct = EditProduct_ComboBox.SelectedItem.ToString();

            //    using (var dbContext = new AppDbContext())
            //    {
            //        var product = dbContext.Product.FirstOrDefault(i => i.ProductName == selectedProduct);

            //        if (product != null)
            //        {
            //            recipeId = product.RecipeId;
            //            NewProductName.Text = product.ProductName;
            //            ProductPrice.Text = product.Price.ToString();
            //            ProductPrice.IsEnabled = true;
            //            ProductPrice.LostFocus -= TextBox_LostFocus;
            //            ProductPrice.GotFocus -= TextBox_GotFocus;
            //            ProductCategory.Text = product.Category;
            //            ProductCategory.IsEnabled = true;
            //            ProductCategory.LostFocus -= TextBox_LostFocus;
            //            ProductCategory.GotFocus -= TextBox_GotFocus;
            //            ProductDescription.Text = product.Description;
            //            ProductDescription.IsEnabled = true;
            //            ProductDescription.LostFocus -= TextBox_LostFocus;
            //            ProductDescription.GotFocus -= TextBox_GotFocus;
            //        }

            //        var recipe = dbContext.Recipes.FirstOrDefault(i => i.RecipeId == recipeId);
            //        if(recipe != null)
            //        {
            //            Recipe.Text = recipe.RecipeContent;
            //            Recipe.IsEnabled = true;
            //            Recipe.LostFocus -= TextBox_LostFocus;
            //            Recipe.GotFocus -= TextBox_GotFocus;
            //        }
            //    }
            //}
        }

        public void EditProduct_Clear()
        {
            //EditProduct_ComboBox.SelectedIndex = 0;
            //ProductPrice.Text = "Cena";
            //ProductPrice.IsEnabled = true;
            //ProductPrice.LostFocus += TextBox_LostFocus;
            //ProductPrice.GotFocus += TextBox_GotFocus;
            //ProductCategory.Text = "Kategoria";
            //ProductCategory.IsEnabled = true;
            //ProductCategory.LostFocus += TextBox_LostFocus;
            //ProductCategory.GotFocus += TextBox_GotFocus;
            //ProductDescription.Text = "Opis Produktu";
            //ProductDescription.IsEnabled = true;
            //ProductDescription.LostFocus += TextBox_LostFocus;
            //ProductDescription.GotFocus += TextBox_GotFocus;
            //Recipe.Text = "Tutaj wpisz przepis na nowego drinka";
            //Recipe.IsEnabled = true;
            //Recipe.LostFocus += TextBox_LostFocus;
            //Recipe.GotFocus += TextBox_GotFocus;
            //NewProductName.Text = "Nazwa nowego produktu";
            //NewProductName.IsEnabled = true;
            //ProductPrice.IsEnabled = true;
        }

        private void CreateNewProduct_CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            //if (CreateNewProduct_CheckBox.IsChecked == true)
            //{
            //    EditProduct_Clear();
            //    EditProduct_ComboBox.IsEnabled = false;
            //}
        }

        private void CreateNewProduct_CheckBoxUnChecked(object sender, RoutedEventArgs e)
        {
            //EditProduct_ComboBox.IsEnabled = true;
            //DeleteIngredient_Button.IsEnabled = true;
            //NewProductName.IsEnabled = false;
            //ProductPrice.IsEnabled = false;
            //ProductCategory.IsEnabled = false;
            //ProductDescription.IsEnabled = false;
            //Recipe.IsEnabled = false;
        }

        private void SaveProduct_ButtonClick(object sender, RoutedEventArgs e)
        {
            //if (CreateNewProduct_CheckBox.IsChecked == true && double.TryParse(ProductPrice.Text, out double price))
            //{
            //    using (var dbContext = new AppDbContext())
            //    {
            //        var newRecipe = new Recipe
            //        {
            //            RecipeName ="Przepis na " + NewProductName.Text,
            //            RecipeContent = Recipe.Text,
            //        };
            //        dbContext.Recipes.Add(newRecipe);
            //        dbContext.SaveChanges();
            //        var newProduct = new Product
            //        {
            //            RecipeId = newRecipe.RecipeId,
            //            ProductName = NewProductName.Text,
            //            Price = Convert.ToDouble(ProductPrice.Text),
            //            Category = ProductCategory.Text,
            //            Description = ProductDescription.Text
            //        };

            //        dbContext.Product.Add(newProduct);
            //        dbContext.SaveChanges();
            //        EditProduct_Clear();
            //        FillEditProductComboBox(EditProduct_ComboBox);
            //        FillEditProductComboBox(EditRecipeOfProduct_ComboBox);
            //        CreateNewProduct_CheckBox.IsChecked = false;
            //        MessageBox.Show("Pomyślnie dodano nowy produkt.");
            //    }
            //}
            //else if(CreateNewProduct_CheckBox.IsChecked == false && double.TryParse(ProductPrice.Text, out price))
            //{
            //    if (EditProduct_ComboBox.SelectedItem != null)
            //    {
            //        string selectedProduct = EditProduct_ComboBox.SelectedItem.ToString();

            //        using (var dbContext = new AppDbContext())
            //        {
            //            int recipeToUpdateId = 0;
            //            var productToUpdate = dbContext.Product.FirstOrDefault(p => p.ProductName == selectedProduct);
            //            if (productToUpdate != null)
            //            {
            //                recipeToUpdateId = productToUpdate.RecipeId;
            //                productToUpdate.ProductName = NewProductName.Text;
            //                productToUpdate.Price = Convert.ToDouble(ProductPrice.Text);
            //                productToUpdate.Category = ProductCategory.Text;
            //                productToUpdate.Description = ProductDescription.Text;
            //            }
            //            var recipeToUpdate = dbContext.Recipes.FirstOrDefault(r => r.RecipeId == recipeToUpdateId);
            //            if(recipeToUpdate != null)
            //            {
            //                recipeToUpdate.RecipeContent = Recipe.Text;
            //            }
            //            dbContext.SaveChanges();
            //            EditProduct_Clear();
            //            CreateNewProduct_CheckBox.IsChecked = false;
            //            MessageBox.Show("Pomyślnie zapisano zmiany.");
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Wybierz produkt do edycji.");
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Wprowadzona cena nie jest cyfrą");
            //}
            //FillEditProductComboBox(EditProduct_ComboBox);
        }

        private void DeleteProduct_ButtonClick(object sender, RoutedEventArgs e)
        {
            //if (EditProduct_ComboBox.SelectedItem != null)
            //{
            //    string selectedProduct = EditProduct_ComboBox.SelectedItem.ToString();

            //    using (var dbContext = new AppDbContext())
            //    {
            //        var product = dbContext.Product.FirstOrDefault(p => p.ProductName == selectedProduct);

            //        if (product != null)
            //        {
            //            int recipeId = product.RecipeId;
            //            int productId = product.ProductId;

            //           try
            //            {
            //                // Usuń RecipeIngredients dla danego przepisu (Recipe_id)
            //                string deleteRecipeIngredientsSql = $"DELETE FROM RecipeIngredients WHERE Recipe_id = {recipeId}";
            //                dbContext.Database.ExecuteSqlRaw(deleteRecipeIngredientsSql);

            //                // Usuń produkt (Product) dla danego produktu (Product_id)
            //                string deleteProductSql = $"DELETE FROM Products WHERE Product_id = {productId}";
            //                dbContext.Database.ExecuteSqlRaw(deleteProductSql);

            //                // Usuń przepis (Recipe) dla danego przepisu (Recipe_id)
            //                string deleteRecipeSql = $"DELETE FROM Recipes WHERE Recipe_id = {recipeId}";
            //                dbContext.Database.ExecuteSqlRaw(deleteRecipeSql);
            //                dbContext.SaveChanges();

            //                MessageBox.Show("Produkt usunięty pomyślnie.");
            //                EditProduct_Clear();
            //                FillEditProductComboBox(EditProduct_ComboBox);
            //                EditProduct_ComboBox.SelectedIndex = 0;
            //                FillEditProductComboBox(EditRecipeOfProduct_ComboBox);
            //                EditRecipeOfProduct_ComboBox.SelectedIndex = 0;
            //            }
            //            catch (Exception ex)
            //            {
            //               MessageBox.Show($"Wystąpił błąd: {ex.Message}");
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Wybierz produkt do usunięcia.");
            //}
        }

        #endregion

        #region right side

        private void FillEditIngredientComboBox(ComboBox ComboBoxName)
        {
            //using (var dbContext = new AppDbContext())
            //{
            //    var ingredients = dbContext.Ingredients.Select(i => i.Name).ToList();

            //    foreach (var ingredient in ingredients)
            //    {
            //        ComboBoxName.Items.Add(ingredient);
            //    }
            //}
        }

        private void EditIngredient_ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (EditIngredient_ComboBox.SelectedItem != null)
            //{
            //    string selectedIngredient = EditIngredient_ComboBox.SelectedItem.ToString();

            //    using (var dbContext = new AppDbContext())
            //    {
            //        var ingredient = dbContext.Ingredients.FirstOrDefault(i => i.Name == selectedIngredient);

            //        if (ingredient != null)
            //        {
            //            NewIngredientName.Text = ingredient.Name;
            //            IngredientUnit.Text = ingredient.Unit;
            //            IngredientPackage.Text = ingredient.Package;
            //            IngredientPackage.IsEnabled = true;
            //            IngredientPackage.LostFocus -= TextBox_LostFocus;
            //            IngredientPackage.GotFocus -= TextBox_GotFocus;
            //            IngredientDescription.Text = ingredient.Description;
            //            IngredientDescription.IsEnabled = true;
            //            IngredientDescription.LostFocus -= TextBox_LostFocus;
            //            IngredientDescription.GotFocus -= TextBox_GotFocus;
            //        }
            //    }
            //}
        }
        public void EditIngredient_Clear()
        {
            //EditIngredient_ComboBox.SelectedIndex = 0;
            //IngredientPackage.Text = "Opakowanie";
            //IngredientPackage.LostFocus += TextBox_LostFocus;
            //IngredientPackage.GotFocus += TextBox_GotFocus;
            //IngredientPackage.IsEnabled = true;
            //IngredientDescription.Text = "Opis składnika";
            //IngredientDescription.LostFocus += TextBox_LostFocus;
            //IngredientDescription.GotFocus += TextBox_GotFocus;
            //IngredientDescription.IsEnabled = true;
            //NewIngredientName.Text = "Nazwa nowego składnika";
            //NewIngredientName.IsEnabled = true;
            //IngredientUnit.IsEnabled = true;
            //DeleteIngredient_Button.IsEnabled = false;
        }
        private void CreateNewIngredient_CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            //if(CreateNewIngredient_CheckBox.IsChecked == true)
            //{
            //    EditIngredient_Clear();
            //    EditIngredient_ComboBox.IsEnabled = false;
            //}
        }
        private void CreateNewIngredient_CheckBoxUnChecked(object sender, RoutedEventArgs e)
        {
            //EditIngredient_ComboBox.IsEnabled = true;
            //DeleteIngredient_Button.IsEnabled = true;
            //NewIngredientName.IsEnabled = false;
            //IngredientUnit.IsEnabled = false;
            //IngredientPackage.IsEnabled = false;
            //IngredientDescription.IsEnabled = false;
        }

        private void SaveIngredient_ButtonClick(object sender, RoutedEventArgs e)
        {
            //if (CreateNewIngredient_CheckBox.IsChecked == true)
            //{
            //    using (var dbContext = new AppDbContext())
            //    {
            //        var newIngredient = new Ingredient
            //        {
            //            Name = NewIngredientName.Text,
            //            Unit = IngredientUnit.Text,
            //            Package = IngredientPackage.Text,
            //            Description = IngredientDescription.Text
            //        };

            //        dbContext.Ingredients.Add(newIngredient);
            //        dbContext.SaveChanges();
            //        EditIngredient_Clear();
            //        FillEditIngredientComboBox(EditIngredient_ComboBox);
            //        FillEditIngredientComboBox(EditRecipeIngredient_ComboBox);
            //        CreateNewIngredient_CheckBox.IsChecked = false;
            //        MessageBox.Show("Pomyślnie dodano nowy składnik.");
            //    }
            //}
            //else
            //{
            //    if (EditIngredient_ComboBox.SelectedItem != null)
            //    {
            //        string selectedIngredient = EditIngredient_ComboBox.SelectedItem.ToString();

            //        using (var dbContext = new AppDbContext())
            //        {
            //            var ingredientToUpdate = dbContext.Ingredients.FirstOrDefault(i => i.Name == selectedIngredient);
            //            if (ingredientToUpdate != null)
            //            {
            //                ingredientToUpdate.Name = NewIngredientName.Text;
            //                ingredientToUpdate.Unit = IngredientUnit.Text;
            //                ingredientToUpdate.Package = IngredientPackage.Text;
            //                ingredientToUpdate.Description = IngredientDescription.Text;

            //                dbContext.SaveChanges();
            //                EditIngredient_Clear();
            //                CreateNewIngredient_CheckBox.IsChecked = false;
            //                MessageBox.Show("Pomyślnie zapisano zmiany.");
            //            }
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Wybierz składnik do edycji.");
            //    }
            //}
            //FillEditIngredientComboBox(EditIngredient_ComboBox);
        }

        private void DeleteIngredient_ButtonClick(object sender, RoutedEventArgs e)
        {
            //if (EditIngredient_ComboBox.SelectedItem != null)
            //{
            //    string selectedIngredient = EditIngredient_ComboBox.SelectedItem.ToString();

            //    using (var dbContext = new AppDbContext())
            //    {
            //        var ingredient = dbContext.Ingredients.FirstOrDefault(i => i.Name == selectedIngredient);

            //        if (ingredient != null)
            //        {
            //            int ingredientId = ingredient.IngredientId;

            //            try
            //            {
            //                // Usuń wszystkie RecipeIngredients dla danego składnika
            //                string deleteRecipeIngredientsSql = $"DELETE FROM RecipeIngredients WHERE Ingredient_id = {ingredientId}";
            //                dbContext.Database.ExecuteSqlRaw(deleteRecipeIngredientsSql);

            //                // Usuń składnik z tabeli Ingredients
            //                string deleteIngredientSql = $"DELETE FROM Ingredients WHERE Ingredient_id = {ingredientId}";
            //                dbContext.Database.ExecuteSqlRaw(deleteIngredientSql);

            //                MessageBox.Show("Składnik usunięty pomyślnie.");
            //                FillEditIngredientComboBox(EditIngredient_ComboBox);
            //                EditIngredient_ComboBox.SelectedIndex = 0;
            //                EditIngredient_Clear();
            //                FillEditIngredientComboBox(EditRecipeIngredient_ComboBox);
            //                EditRecipeIngredient_ComboBox.SelectedIndex = 0;
            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show($"Wystąpił błąd: {ex.Message}");
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Wybierz składnik do usunięcia.");
            //}
        }

        #endregion
    }
}
