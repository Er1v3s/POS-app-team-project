using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.SalesPanel;

namespace POS.Views.Windows.SalesPanel
{
    /// <summary>
    /// Logika interakcji dla klasy SalesPanel.xaml
    /// </summary>
    public partial class SalesPanelWindow : Window
    {
        public int EmployeeId;

        //ObservableCollection<OrderItemDto> orderList = [];
        //ObservableCollection<ObservableCollection<OrderItemDto>> orderListCollection = [];

        public SalesPanelWindow(int employeeId)
        {
            //orderListCollection.Add(orderList);

            //
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<SalesPanelViewModel>();

            var viewModel = (SalesPanelViewModel)DataContext;
            viewModel.CloseWindowBaseAction = Close;
            //

            //LoadAllProducts();
            //orderListDataGrid.ItemsSource = orderListCollection[currentOrderId];
            //UpdateTotalPrice();
            EmployeeId = employeeId;
        }

        private void RemoveIngredients()
        {
            //using (var dbContext = new AppDbContext())
            //{
            //    foreach (var item in orderList)
            //    {
            //        var recipeId = dbContext.Product
            //                                .Where(p => p.ProductId == item.ProductId)
            //                                .Select(p => p.RecipeId)
            //                                .FirstOrDefault();
            //        if (recipeId == null)
            //        {
            //            throw new Exception("Nie znaleziono przepisu dla danego produktu");
            //        }

            //        var recipeIngredientsId = dbContext.RecipeIngredients
            //                .Where(ri => ri.RecipeId == recipeId)
            //                .Select(ri => ri.IngredientId)
            //                .ToList();

            //        foreach(var ingredientId in recipeIngredientsId)
            //        {
            //            var ingredient = dbContext.Ingredients
            //                            .Where(i => i.IngredientId == ingredientId)
            //                            .FirstOrDefault();

            //            var recipeIngredient = dbContext.RecipeIngredients
            //                                    .Where(ri => ri.IngredientId == ingredientId)
            //                                    .Select(ri => ri.Quantity)
            //                                    .FirstOrDefault();

            //            if (ingredient != null)
            //            {
            //                // Tak funkcja prezentuje się w poprawny sposób, ale trzeba zmienić sposób przedstawiania ilości składników w bazie danych 
            //                //ingredient.Stock -= (int)recipeIngredient;

            //                // Tymczasowe, po zmianie wartości w bazie danych usunąć!!! 
            //                ingredient.Stock -= 1;
            //            }
            //            else
            //            {
            //                throw new Exception("Składnik nie znaleziony w magazynie.");
            //            }
            //        }
                    
            //    }

            //    dbContext.SaveChanges();
            //}
        }

        private void AddInvoice_ButtonClick(object sender, RoutedEventArgs e)
        {
            //if (orderListCollection[currentOrderId].Count == 0)
            //{
            //    MessageBox.Show("Brak produktów do dodania do faktury.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //InvoiceWindow invoiceWindow = new InvoiceWindow();
            //if (invoiceWindow.ShowDialog() == true)
            //{
            //    InvoiceCustomerDataDto invoiceCustomerData = InvoiceWindow.InvoiceCustomerDataObject;
            //    MessageBox.Show($"Faktura dla: {invoiceCustomerData.CustomerName}\nAdres: {invoiceCustomerData.CustomerAddress}\nNIP: {invoiceCustomerData.TaxIdentificationNumber}\nSuma: {totalPrice:C2}", "Faktura", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }
    }
}
