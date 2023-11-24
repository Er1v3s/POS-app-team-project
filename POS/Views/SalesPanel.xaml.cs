using Microsoft.EntityFrameworkCore;
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
    /// Logika interakcji dla klasy SalesPanel.xaml
    /// </summary>
    public partial class SalesPanel
    {
        private double totalPrice = 0;
        ObservableCollection<OrderItem> orderList = new ObservableCollection<OrderItem>();

        public SalesPanel()
        {
            InitializeComponent();
            LoadAllProducts();
            orderListDataGrid.ItemsSource = orderList;
            UpdateTotalPrice();
        }

        private void MoveToMainWindow(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            Window.GetWindow(this).Close();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text.Length > 0)
            {
                searchTextBox.Text = "";
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                searchTextBox.Text = "Szukaj";
            }
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var searchText = searchTextBox.Text.ToLower();
                LoadProductsBySearch(searchText);

                e.Handled = true;
                searchTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            Button categoryButton = sender as Button;
            string category = categoryButton.Content.ToString();
            LoadProductsByCategory(category);
        }

        private void UpdateTotalPrice()
        {
            totalPrice = orderList.Sum(item => item.Amount * item.Price);
            totalAmountLabel.Content = $"{totalPrice:C2}";
        }


        private void AddOrUpdateProductInList(Products product)
        {
            var existingProduct = orderList.FirstOrDefault(p => p.Id == product.Product_id);

            if (existingProduct != null)
            {
                existingProduct.Amount++;
            }
            else
            {
                orderList.Add(new OrderItem { Id = product.Product_id, Name = product.Product_name, Amount = 1, Price = Convert.ToDouble(product.Price) });
            }
        }


        private void DeleteProductFromOrderList(object sender, RoutedEventArgs e)
        {
            if (orderListDataGrid.SelectedItem != null)
            {
                var selectedItem = (OrderItem)orderListDataGrid.SelectedItem;
                selectedItem.Amount--;

                if (selectedItem.Amount == 0)
                {
                    orderList.Remove(selectedItem);
                }

                UpdateTotalPrice();
            }
        }

        private Button CreateProductButton(Products product)
        {
            Viewbox viewbox = new Viewbox();
            Button button = new Button
            {
                Style = (Style)FindResource("chooseProductButton"),
                Content = new StackPanel
                {
                    Children =
            {
                new TextBlock { TextAlignment = TextAlignment.Center, Margin = new Thickness(10), Text = product.Product_name },
                new TextBlock { TextAlignment = TextAlignment.Center, Margin = new Thickness(10), Text = $"{product.Price} zł" }
            }
                }
            };

            // button onClick:
            button.Click += (object sender, RoutedEventArgs e) =>
            {
                AddOrUpdateProductInList(product);
                UpdateTotalPrice();
            };

            ProductsUnifromGrid.Children.Add(viewbox);
            viewbox.Child = button;

            return button;
        }

        private void LoadProducts(IEnumerable<Products> products)
        {
            ProductsUnifromGrid.Children.Clear();

            foreach (var product in products)
            {
                CreateProductButton(product);
            }
        }

        private void LoadAllProducts()
        {
            using (var dbContext = new AppDbContext())
            {
                var products = dbContext.Products.ToList();
                LoadProducts(products);
            }
        }

        private void LoadProductsBySearch(string searchText)
        {
            using (var dbContext = new AppDbContext())
            {
                var products = dbContext.Products.Where(p => p.Product_name.ToLower().Contains(searchText)).ToList();
                LoadProducts(products);
            }
        }

        private void LoadProductsByCategory(string category)
        {
            using (var dbContext = new AppDbContext())
            {
                var products = dbContext.Products.Where(p => p.Category == category).ToList();
                LoadProducts(products);
            }
        }
        private void ShowRecipes(object sender, RoutedEventArgs e)
        {
            ProductsWrapPanel.Children.Clear();

            if (orderList.Count == 0)
            {
                TextBlock emptyListTextBlock = new TextBlock
                {
                    Style = (Style)FindResource("RecipeTextStyle"),
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(10),
                    TextWrapping = TextWrapping.Wrap,
                    Text = "Dodaj produkt do zamówienia aby podejrzeć przepis"
                };

                ProductsWrapPanel.Children.Add(emptyListTextBlock);
            }
            else
            {
                foreach (var orderItem in orderList)
                {
                    string productName = orderItem.Name;
                    string recipeForProduct = GetRecipe(productName);
                    string recipeIngredients = GetRecipeIngredients(productName);

                    TextBlock textBlock = new TextBlock
                    {
                        Style = (Style)FindResource("RecipeTextStyle"),
                        TextAlignment = TextAlignment.Left,
                        Margin = new Thickness(10),
                        TextWrapping = TextWrapping.Wrap,
                        Text = productName + "\n\n" + recipeIngredients + "\n" + recipeForProduct
                    };

                    ProductsWrapPanel.Children.Add(textBlock);
                }
            }
        }


        private string GetRecipe(string productName)
        {
            string recipe = "";

            using (var dbContext = new AppDbContext())
            {
                var queryResult = dbContext.Products
                    .Join(dbContext.Recipes, p => p.Recipe_id, r => r.Recipe_id, (p, r) => new { p, r })
                    .Where(join => join.p.Product_name == productName)
                    .Select(join => join.r.Recipe)
                    .FirstOrDefault();

                if (queryResult != null)
                {
                    recipe = queryResult.ToString();
                }
            }

            return recipe;
        }
        private string GetRecipeIngredients(string productName)
        {
            StringBuilder ingredientsList = new StringBuilder();

            using (var dbContext = new AppDbContext())
            {
                var queryResult = dbContext.Products
                    .Join(dbContext.Recipes, p => p.Recipe_id, r => r.Recipe_id, (p, r) => new { p, r })
                    .Join(dbContext.RecipeIngredients, j => j.r.Recipe_id, ri => ri.Recipe_id, (j, ri) => new { j, ri })
                    .Join(dbContext.Ingredients, jri => jri.ri.Ingredient_id, i => i.Ingredient_id, (jri, i) => new { jri, i })
                    .Where(join => join.jri.j.p.Product_name == productName)
                    .Select(join => new
                    {
                        IngredientName = join.i.Name,
                        IngredientDescription = join.i.Description,
                        IngredientUnit = join.i.Unit,
                        IngredientQuantity = join.jri.ri.Quantity
                    });

                foreach (var ingredient in queryResult)
                {
                    ingredientsList.AppendLine($"{ingredient.IngredientName} - {ingredient.IngredientQuantity} {ingredient.IngredientUnit}");
                }
            }

            return ingredientsList.ToString();
        }
        private void ShowDrinks(object sender, RoutedEventArgs e)
        {
            ProductsWrapPanel.Children.Clear();
            LoadProducts();
        }

    }
}
