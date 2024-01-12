using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
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
using static iTextSharp.text.pdf.AcroFields;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy CreateDelivery.xaml
    /// </summary>
    public partial class CreateDelivery
    {
        private Employees currentUser;
        public int EmployeeId;
        List<DeliveryItem> deliveryItems = new List<DeliveryItem>();
        public CreateDelivery(int employeeId)
        {
            InitializeComponent();
            using (var dbContext = new AppDbContext())
            {
                var ingredients = dbContext.Ingredients.ToList();
                IngredientsDataGrid.ItemsSource = ingredients;
                currentUser = dbContext.Employees.FirstOrDefault(e => e.Employee_id == employeeId);
            }
            string welcomeMessage = $"{currentUser.First_name} {currentUser.Last_name}";
            SetWelcomeMessage(welcomeMessage);
            EmployeeId = employeeId;
            deliveryListDataGrid.ItemsSource = deliveryItems;
        }
        public CreateDelivery()
        {
            InitializeComponent();
        }

        private void MoveToMainWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            Window.GetWindow(this).Close();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderTextBoxHelper.SetPlaceholderOnFocus(sender, e);
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderTextBoxHelper.SetPlaceholderOnLostFocus(sender, e);
        }
        private void SetWelcomeMessage(string message)
        {
            welcomeLabel.Content = message;
        }

        private void AddToDeliveryButton_Click(object sender, RoutedEventArgs e)
        {
            Ingredients selectedIngredient = (Ingredients)IngredientsDataGrid.SelectedItem;
            if (selectedIngredient != null)
            {
                // Wyświetl MessageBox z pytaniem o nową wartość
                string newValueString = Microsoft.VisualBasic.Interaction.InputBox("Enter new value:", "Edit Value", selectedIngredient.Stock.ToString());

                // Sprawdź, czy użytkownik nie anulował operacji
                if (!string.IsNullOrEmpty(newValueString))
                {
                    // Spróbuj przekształcić wprowadzoną wartość na int
                    if (int.TryParse(newValueString, out int newValue))
                    {
                        // Tutaj użyj zapytania SQLite do zaktualizowania wartości w tabeli Ingredients
                        using (var dbContext = new AppDbContext())
                        {
                            var ingredientToUpdate = dbContext.Ingredients.FirstOrDefault(i => i.Ingredient_id == selectedIngredient.Ingredient_id);

                            if (ingredientToUpdate != null)
                            {
                                DeliveryItem newItem = new DeliveryItem
                                {
                                    Name = ingredientToUpdate.Name,
                                    Quantity = newValue,
                                    Package = ingredientToUpdate.Package,
                                    FullQuantity = newValue + ingredientToUpdate.Safety_stock // in future there could be a calculated quantity of packages to order.
                                };

                                deliveryItems.Add(newItem);
                                deliveryListDataGrid.ItemsSource = null;
                                deliveryListDataGrid.ItemsSource = deliveryItems;
                            }
                        }

                        // Odśwież dane w DataGrid
                        RefreshIngredientsDataGrid();
                    }
                    else
                    {
                        MessageBox.Show("Invalid input. Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void EditValueButton_Click(object sender, RoutedEventArgs e)
        {
            // Pobierz wybrany składnik z DataGrid
            Ingredients selectedIngredient = (Ingredients)IngredientsDataGrid.SelectedItem;

            if (selectedIngredient != null)
            {
                // Wyświetl MessageBox z pytaniem o nową wartość
                string newValueString = Microsoft.VisualBasic.Interaction.InputBox("Enter new value:", "Edit Value", selectedIngredient.Stock.ToString());

                // Sprawdź, czy użytkownik nie anulował operacji
                if (!string.IsNullOrEmpty(newValueString))
                {
                    // Spróbuj przekształcić wprowadzoną wartość na int
                    if (int.TryParse(newValueString, out int newValue))
                    {
                        // Tutaj użyj zapytania SQLite do zaktualizowania wartości w tabeli Ingredients
                        using (var dbContext = new AppDbContext())
                        {
                            var ingredientToUpdate = dbContext.Ingredients.FirstOrDefault(i => i.Ingredient_id == selectedIngredient.Ingredient_id);

                            if (ingredientToUpdate != null)
                            {
                                ingredientToUpdate.Stock = newValue;
                                dbContext.SaveChanges();
                            }
                        }

                        // Odśwież dane w DataGrid
                        RefreshIngredientsDataGrid();
                    }
                    else
                    {
                        MessageBox.Show("Invalid input. Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void AddFromDeliveryButton_Click(Object sender, RoutedEventArgs e) { }

        private void RefreshIngredientsDataGrid()
        {
            // Ponownie pobierz dane i ustaw jako źródło dla DataGrid
            using (var dbContext = new AppDbContext())
            {
                var ingredients = dbContext.Ingredients.ToList();
                IngredientsDataGrid.ItemsSource = ingredients;
            }
        }

    }

    public class DeliveryItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Package { get; set; }
        public int FullQuantity { get; set; }
    }
}
