using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using POS.Converter;
using POS.Models;
using POS.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Paragraph = iTextSharp.text.Paragraph;

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
                string newValueString = Microsoft.VisualBasic.Interaction.InputBox("Podaj ilość składnika:", "Dodaj składnik do zamówienia", selectedIngredient.Stock.ToString());

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

        private void GenerateDeliveryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Pliki PDF (*.pdf)|*.pdf";
                saveFileDialog.FileName = "Podsumowanie_zamowienia.pdf";

                bool? result = saveFileDialog.ShowDialog();

                if (result == true)
                {
                    string filePath = saveFileDialog.FileName;

                    Document pdfDoc = new Document(PageSize.A4);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, new FileStream(filePath, FileMode.Create));

                    pdfDoc.Open();
                    pdfDoc.NewPage();

                    PdfPTable pdfTable = new PdfPTable(deliveryListDataGrid.Columns.Count);

                    foreach (DataGridColumn column in deliveryListDataGrid.Columns)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(column.Header.ToString()));
                        pdfTable.AddCell(cell);
                    }

                    foreach (var item in deliveryListDataGrid.Items)
                    {
                        foreach (DataGridColumn column in deliveryListDataGrid.Columns)
                        {
                            string cellValue = (column.GetCellContent(item) as TextBlock)?.Text;
                            PdfPCell cell = new PdfPCell(new Phrase(cellValue ?? ""));
                            pdfTable.AddCell(cell);
                        }
                    }

                    Paragraph pdfTitle = new Paragraph("Zamówienie", new Font(Font.FontFamily.TIMES_ROMAN, 18, Font.BOLD));
                    pdfTitle.Alignment = Element.ALIGN_CENTER;
                    pdfTitle.SpacingAfter = 10f;
                    pdfTitle.SetLeading(0, 1.2f);

                    pdfDoc.Add(pdfTitle);
                    pdfDoc.Add(pdfTable);

                    pdfDoc.Close();
                    this.Close();
                    MessageBox.Show("Zamówienie zostało zapisane do pliku PDF.");
                }
                else
                {
                    MessageBox.Show("Anulowano zapisywanie pliku PDF.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas zapisywania pliku PDF: " + ex.Message);
            }
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                using (var dbContext = new AppDbContext())
                {
                    string searchPhrase = searchTextBox.Text.ToLower();

                    var filteredIngredients = dbContext.Ingredients
                        .Where(ingredient => ingredient.Name.ToLower().Contains(searchPhrase))
                        .ToList();

                    if(filteredIngredients.Count != 0)
                    {
                        IngredientsDataGrid.ItemsSource = filteredIngredients;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas filtrowania składników: " + ex.Message);
            }
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                row.IsSelected = !row.IsSelected;
                e.Handled = true;
            }
        }
    }
}
