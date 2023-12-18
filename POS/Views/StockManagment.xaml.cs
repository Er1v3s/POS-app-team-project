using POS.Converter;
using POS.Models;
using POS.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy StockManagment.xaml
    /// </summary>
    public partial class StockManagment
    {
        private Employees currentUser;
        public int EmployeeId;
        public StockManagment(int employeeId)
        {
            InitializeComponent();
            FillEditIngredientComboBox(EditIngredient_ComboBox);
            FillEditProductComboBox(EditProduct_ComboBox);
            FillEditIngredientComboBox(EditRecipeIngredient_ComboBox);
            FillEditProductComboBox(EditRecipeOfProduct_ComboBox);

            using (var dbContext = new AppDbContext())
            {
                currentUser = dbContext.Employees.FirstOrDefault(e => e.Employee_id == employeeId);
            }
            string welcomeMessage = $"{currentUser.First_name} {currentUser.Last_name}";
            SetWelcomeMessage(welcomeMessage);
            EmployeeId = employeeId;
        }
        private void MoveToMainWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            Window.GetWindow(this).Close();
        }
        private void SetWelcomeMessage(string message)
        {
            welcomeLabel.Content = message;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderTextBoxHelper.SetPlaceholderOnFocus(sender, e);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderTextBoxHelper.SetPlaceholderOnLostFocus(sender, e);
        }

        // left side
        private void EditRecipeOfProduct_ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EditRecipeOfProduct_ComboBox.SelectedItem != null)
            {
                string selectedProduct = EditRecipeOfProduct_ComboBox.SelectedItem.ToString();

                using (var dbContext = new AppDbContext())
                {
                    var product = dbContext.Products.FirstOrDefault(i => i.Product_name == selectedProduct);

                    if (product != null)
                    {
                        var ingredients = dbContext.RecipeIngredients
                            .Where(ri => ri.Recipe_id == product.Recipe_id)
                            .Join(dbContext.Ingredients,
                                ri => ri.Ingredient_id,
                                i => i.Ingredient_id,
                                (ri, i) => new
                                {
                                    ri.RecipeIngredient_id,
                                    ri.Ingredient_id,
                                    ri.Quantity,
                                    i.Name,
                                    i.Unit
                                })
                            .ToList();

                        recipeIngredientsDataGrid.ItemsSource = ingredients;
                        EditRecipeIngredient_ComboBox.IsEnabled = true;
                    }
                }
            }
        }
        
        private void EditRecipeIngredient_ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(EditRecipeIngredient_ComboBox != null)
            {
                string selectedIngredient = EditRecipeIngredient_ComboBox.SelectedItem.ToString();

                using (var dbContext = new AppDbContext())
                {
                    var recipeIngredient = dbContext.Ingredients.FirstOrDefault(i => i.Name == selectedIngredient);
                    if(recipeIngredient != null)
                    {
                        RecipeIngredientQuantity.IsEnabled = true;
                        EditRecipeOfProduct_ComboBox.IsEnabled = false;
                    }
                }
            }
        }

        private void DeleteRecipeIngredient_ButtonClick(object sender, RoutedEventArgs e)
        {
            int IngredientId = 0, RecipeId = 0;
            string selectedIngredient = EditRecipeIngredient_ComboBox.SelectedItem.ToString();
            string selectedProduct = EditRecipeOfProduct_ComboBox.SelectedItem.ToString();

            using (var dbContext = new AppDbContext())
            {
                var ingredient = dbContext.Ingredients.FirstOrDefault(i => i.Name == selectedIngredient);
                if (ingredient != null)
                {
                    IngredientId = ingredient.Ingredient_id;
                }

                var product = dbContext.Products.FirstOrDefault(p => p.Product_name == selectedProduct);
                if (product != null)
                {
                    RecipeId = product.Recipe_id;
                }

                // błąd tabela RecipeIngredients nie zawiera kolumny "Ingredient_id1"
                var recipeIngredient = dbContext.RecipeIngredients.FirstOrDefault(ri => ri.Ingredient_id == IngredientId && ri.Recipe_id == RecipeId);
                if (recipeIngredient != null)
                {
                    dbContext.RecipeIngredients.Remove(recipeIngredient);
                    dbContext.SaveChanges();
                    MessageBox.Show("Rekord usunięty pomyślnie.");
                }
                else
                {
                    MessageBox.Show("Nie znaleziono rekordu do usunięcia.");
                }
            }
        }
        private void AddRecipeIngredient_ButtonClick(object sender, RoutedEventArgs e)
        {
            int IngredientId = 0, RecipeId = 0;
            string selectedIngredient = EditRecipeIngredient_ComboBox.SelectedItem.ToString();
            string selectedProduct = EditRecipeOfProduct_ComboBox.SelectedItem.ToString();
            int Quantity = Convert.ToInt32(RecipeIngredientQuantity.Text);

            using (var dbContext = new AppDbContext())
            {
                var product = dbContext.Products.FirstOrDefault(p => p.Product_name == selectedProduct);
                if (product != null)
                {
                    RecipeId = product.Recipe_id;
                }

                var recipe = dbContext.Recipes.FirstOrDefault(r => r.Recipe_id == RecipeId);
                var ingredient = dbContext.Ingredients.FirstOrDefault(i => i.Name == selectedIngredient);
                if (ingredient != null && recipe != null)
                {

                    var recipeIngredient = new RecipeIngredients
                    {
                        Recipe = recipe,
                        Ingredient = ingredient,
                        Quantity = Quantity
                    };

                    dbContext.RecipeIngredients.Add(recipeIngredient);
                    dbContext.SaveChanges();

                    MessageBox.Show("Rekord dodany pomyślnie.");
                }
            }
        }


        // middle side
        private void FillEditProductComboBox(ComboBox ComboBoxName)
        {
            using (var dbContext = new AppDbContext())
            {
                var products = dbContext.Products.Select(i => i.Product_name).ToList();

                foreach (var product in products)
                {
                    ComboBoxName.Items.Add(product);
                }
            }
        }
        private void EditProduct_ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EditProduct_ComboBox.SelectedItem != null)
            {
                int recipeId = 0;
                string selectedProduct = EditProduct_ComboBox.SelectedItem.ToString();

                using (var dbContext = new AppDbContext())
                {
                    var product = dbContext.Products.FirstOrDefault(i => i.Product_name == selectedProduct);

                    if (product != null)
                    {
                        recipeId = product.Recipe_id;
                        NewProductName.Text = product.Product_name;
                        ProductPrice.Text = product.Price.ToString();
                        ProductPrice.IsEnabled = true;
                        ProductPrice.LostFocus -= TextBox_LostFocus;
                        ProductPrice.GotFocus -= TextBox_GotFocus;
                        ProductCategory.Text = product.Category;
                        ProductCategory.IsEnabled = true;
                        ProductCategory.LostFocus -= TextBox_LostFocus;
                        ProductCategory.GotFocus -= TextBox_GotFocus;
                        ProductDescription.Text = product.Description;
                        ProductDescription.IsEnabled = true;
                        ProductDescription.LostFocus -= TextBox_LostFocus;
                        ProductDescription.GotFocus -= TextBox_GotFocus;
                    }

                    var recipe = dbContext.Recipes.FirstOrDefault(i => i.Recipe_id == recipeId);
                    if(recipe != null)
                    {
                        Recipe.Text = recipe.Recipe;
                        Recipe.IsEnabled = true;
                        Recipe.LostFocus -= TextBox_LostFocus;
                        Recipe.GotFocus -= TextBox_GotFocus;
                    }
                }
            }
        }
        public void EditProduct_Clear()
        {
            EditProduct_ComboBox.SelectedIndex = 0;
            ProductPrice.Text = "Cena";
            ProductPrice.IsEnabled = true;
            ProductPrice.LostFocus += TextBox_LostFocus;
            ProductPrice.GotFocus += TextBox_GotFocus;
            ProductCategory.Text = "Kategoria";
            ProductCategory.IsEnabled = true;
            ProductCategory.LostFocus += TextBox_LostFocus;
            ProductCategory.GotFocus += TextBox_GotFocus;
            ProductDescription.Text = "Opis Produktu";
            ProductDescription.IsEnabled = true;
            ProductDescription.LostFocus += TextBox_LostFocus;
            ProductDescription.GotFocus += TextBox_GotFocus;
            Recipe.Text = "Tutaj wpisz przepis na nowego drinka";
            Recipe.IsEnabled = true;
            Recipe.LostFocus += TextBox_LostFocus;
            Recipe.GotFocus += TextBox_GotFocus;
            NewProductName.Text = "Nazwa nowego produktu";
            NewProductName.IsEnabled = true;
            ProductPrice.IsEnabled = true;
        }
        private void CreateNewProduct_CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            if (CreateNewProduct_CheckBox.IsChecked == true)
            {
                EditProduct_Clear();
                EditProduct_ComboBox.IsEnabled = false;
            }
        }
        private void CreateNewProduct_CheckBoxUnChecked(object sender, RoutedEventArgs e)
        {
            EditProduct_ComboBox.IsEnabled = true;
            DeleteIngredient_Button.IsEnabled = true;
            NewProductName.IsEnabled = false;
            ProductPrice.IsEnabled = false;
            ProductCategory.IsEnabled = false;
            ProductDescription.IsEnabled = false;
            Recipe.IsEnabled = false;
        }

        private void SaveProduct_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (CreateNewProduct_CheckBox.IsChecked == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    var newRecipe = new Recipes
                    {
                        Recipe_name ="Przepis na " + NewProductName.Text,
                        Recipe = Recipe.Text,
                    };
                    dbContext.Recipes.Add(newRecipe);
                    dbContext.SaveChanges();
                    var newProduct = new Products
                    {
                        Recipe_id = newRecipe.Recipe_id,
                        Product_name = NewProductName.Text,
                        Price = Convert.ToDouble(ProductPrice.Text),
                        Category = ProductCategory.Text,
                        Description = ProductDescription.Text
                    };

                    dbContext.Products.Add(newProduct);
                    dbContext.SaveChanges();
                    EditProduct_Clear();
                    CreateNewProduct_CheckBox.IsChecked = false;
                    MessageBox.Show("Pomyślnie dodano nowy produkt.");
                }
            }
            else
            {
                if (EditProduct_ComboBox.SelectedItem != null)
                {
                    string selectedProduct = EditProduct_ComboBox.SelectedItem.ToString();

                    using (var dbContext = new AppDbContext())
                    {
                        int recipeToUpdateId = 0;
                        var productToUpdate = dbContext.Products.FirstOrDefault(p => p.Product_name == selectedProduct);
                        if (productToUpdate != null)
                        {
                            recipeToUpdateId = productToUpdate.Recipe_id;
                            productToUpdate.Product_name = NewProductName.Text;
                            productToUpdate.Price = Convert.ToDouble(ProductPrice.Text);
                            productToUpdate.Category = ProductCategory.Text;
                            productToUpdate.Description = ProductDescription.Text;
                        }
                        var recipeToUpdate = dbContext.Recipes.FirstOrDefault(r => r.Recipe_id == recipeToUpdateId);
                        if(recipeToUpdate != null)
                        {
                            recipeToUpdate.Recipe = Recipe.Text;
                        }
                        dbContext.SaveChanges();
                        EditProduct_Clear();
                        CreateNewProduct_CheckBox.IsChecked = false;
                        MessageBox.Show("Pomyślnie zapisano zmiany.");
                    }
                }
                else
                {
                    MessageBox.Show("Wybierz produkt do edycji.");
                }
            }
            FillEditProductComboBox(EditProduct_ComboBox);
        }


        // right side
        private void FillEditIngredientComboBox(ComboBox ComboBoxName)
        {
            using (var dbContext = new AppDbContext())
            {
                var ingredients = dbContext.Ingredients.Select(i => i.Name).ToList();

                foreach (var ingredient in ingredients)
                {
                    ComboBoxName.Items.Add(ingredient);
                }
            }
        }

        private void EditIngredient_ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EditIngredient_ComboBox.SelectedItem != null)
            {
                string selectedIngredient = EditIngredient_ComboBox.SelectedItem.ToString();

                using (var dbContext = new AppDbContext())
                {
                    var ingredient = dbContext.Ingredients.FirstOrDefault(i => i.Name == selectedIngredient);

                    if (ingredient != null)
                    {
                        NewIngredientName.Text = ingredient.Name;
                        IngredientUnit.Text = ingredient.Unit;
                        IngredientPackage.Text = ingredient.Package;
                        IngredientPackage.IsEnabled = true;
                        IngredientPackage.LostFocus -= TextBox_LostFocus;
                        IngredientPackage.GotFocus -= TextBox_GotFocus;
                        IngredientDescription.Text = ingredient.Description;
                        IngredientDescription.IsEnabled = true;
                        IngredientDescription.LostFocus -= TextBox_LostFocus;
                        IngredientDescription.GotFocus -= TextBox_GotFocus;
                    }
                }
            }
        }
        public void EditIngredient_Clear()
        {
            EditIngredient_ComboBox.SelectedIndex = 0;
            IngredientPackage.Text = "Opakowanie";
            IngredientPackage.LostFocus += TextBox_LostFocus;
            IngredientPackage.GotFocus += TextBox_GotFocus;
            IngredientPackage.IsEnabled = true;
            IngredientDescription.Text = "Opis składnika";
            IngredientDescription.LostFocus += TextBox_LostFocus;
            IngredientDescription.GotFocus += TextBox_GotFocus;
            IngredientDescription.IsEnabled = true;
            NewIngredientName.Text = "Nazwa nowego składnika";
            NewIngredientName.IsEnabled = true;
            IngredientUnit.IsEnabled = true;
            DeleteIngredient_Button.IsEnabled = false;
        }
        private void CreateNewIngredient_CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            if(CreateNewIngredient_CheckBox.IsChecked == true)
            {
                EditIngredient_Clear();
                EditIngredient_ComboBox.IsEnabled = false;
            }
        }
        private void CreateNewIngredient_CheckBoxUnChecked(object sender, RoutedEventArgs e)
        {
            EditIngredient_ComboBox.IsEnabled = true;
            DeleteIngredient_Button.IsEnabled = true;
            NewIngredientName.IsEnabled = false;
            IngredientUnit.IsEnabled = false;
            IngredientPackage.IsEnabled = false;
            IngredientDescription.IsEnabled = false;

            // temporary not enabled
            DeleteIngredient_Button.IsEnabled = false;
        }

        private void SaveIngredient_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (CreateNewIngredient_CheckBox.IsChecked == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    var newIngredient = new Ingredients
                    {
                        Name = NewIngredientName.Text,
                        Unit = IngredientUnit.Text,
                        Package = IngredientPackage.Text,
                        Description = IngredientDescription.Text
                    };

                    dbContext.Ingredients.Add(newIngredient);
                    dbContext.SaveChanges();
                    EditIngredient_Clear();
                    CreateNewIngredient_CheckBox.IsChecked = false;
                    MessageBox.Show("Pomyślnie dodano nowy składnik.");
                }
            }
            else
            {
                if (EditIngredient_ComboBox.SelectedItem != null)
                {
                    string selectedIngredient = EditIngredient_ComboBox.SelectedItem.ToString();

                    using (var dbContext = new AppDbContext())
                    {
                        var ingredientToUpdate = dbContext.Ingredients.FirstOrDefault(i => i.Name == selectedIngredient);
                        if (ingredientToUpdate != null)
                        {
                            ingredientToUpdate.Name = NewIngredientName.Text;
                            ingredientToUpdate.Unit = IngredientUnit.Text;
                            ingredientToUpdate.Package = IngredientPackage.Text;
                            ingredientToUpdate.Description = IngredientDescription.Text;

                            dbContext.SaveChanges();
                            EditIngredient_Clear();
                            CreateNewIngredient_CheckBox.IsChecked = false;
                            MessageBox.Show("Pomyślnie zapisano zmiany.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Wybierz składnik do edycji.");
                }
            }
            FillEditIngredientComboBox(EditIngredient_ComboBox);
        }
    }
}
