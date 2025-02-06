using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.WarehouseFunctions;

namespace POS.Views.Windows.WarehouseFunctions
{
    /// <summary>
    /// Logika interakcji dla klasy CreateDelivery.xaml
    /// </summary>
    public partial class CreateDeliveryWindow : Window
    {
        public CreateDeliveryWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<CreateDeliveryViewModel>();

            var viewModel = (CreateDeliveryViewModel)DataContext;
            viewModel.CloseWindowBaseAction = Close;
        }

        private void RefreshIngredientsDataGrid()
        {
            //using (var dbContext = new AppDbContext())
            //{
            //    var ingredients = dbContext.Ingredients.ToList();
            //    IngredientsDataGrid.ItemsSource = ingredients;
            //}
        }

        private void GenerateDelivery_ButtonClick(object sender, RoutedEventArgs e)
        {
            GenerateDeliveryPDFFile();
        }

        private async void GenerateDeliveryPDFFile()
        {
            //await Task.Run(() =>
            //{
            //    try
            //    {
            //        SaveFileDialog saveFileDialog = new SaveFileDialog();
            //        saveFileDialog.Filter = "Pliki PDF (*.pdf)|*.pdf";
            //        saveFileDialog.FileName = "Podsumowanie_zamowienia.pdf";

            //        bool? result = saveFileDialog.ShowDialog();

            //        if (result == true)
            //        {
            //            string filePath = saveFileDialog.FileName;

            //            Document pdfDoc = new Document(PageSize.A4);
            //            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, new FileStream(filePath, FileMode.Create));

            //            pdfDoc.Open();
            //            pdfDoc.NewPage();

            //            PdfPTable pdfTable = new PdfPTable(deliveryListDataGrid.Columns.Count);

            //            foreach (DataGridColumn column in deliveryListDataGrid.Columns)
            //            {
            //                PdfPCell cell = new PdfPCell(new Phrase(column.Header.ToString()));
            //                pdfTable.AddCell(cell);
            //            }

            //            foreach (var item in deliveryListDataGrid.Items)
            //            {
            //                foreach (DataGridColumn column in deliveryListDataGrid.Columns)
            //                {
            //                    string cellValue = (column.GetCellContent(item) as TextBlock)?.Text;
            //                    PdfPCell cell = new PdfPCell(new Phrase(cellValue ?? ""));
            //                    pdfTable.AddCell(cell);
            //                }
            //            }

            //            Paragraph pdfTitle = new Paragraph("Zamówienie", new Font(Font.FontFamily.TIMES_ROMAN, 18, Font.BOLD));
            //            pdfTitle.Alignment = Element.ALIGN_CENTER;
            //            pdfTitle.SpacingAfter = 10f;
            //            pdfTitle.SetLeading(0, 1.2f);

            //            pdfDoc.Add(pdfTitle);
            //            pdfDoc.Add(pdfTable);

            //            pdfDoc.Close();
            //            this.Close();
            //            MessageBox.Show("Zamówienie zostało zapisane do pliku PDF.");
            //        }
            //        else
            //        {
            //            MessageBox.Show("Anulowano zapisywanie pliku PDF.");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("Wystąpił błąd podczas zapisywania pliku PDF: " + ex.Message);
            //    }
            //});
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
