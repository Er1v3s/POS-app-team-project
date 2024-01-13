using iTextSharp.text.pdf;
using iTextSharp.text;
using POS.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy GenerateBill.xaml
    /// </summary>
    public partial class GenerateBill
    {
        private ObservableCollection<OrderItem> orderList;
        double totalOrderPrice = 0;

        public GenerateBill(ObservableCollection<OrderItem> orderList)
        {
            InitializeComponent();

            this.orderList = orderList;

            CalculateProductTimesAmount();
            CalculateTotalPrice(ref totalOrderPrice);

            orderSummaryDataGrid.ItemsSource = this.orderList;
            totalPriceTextBlock.Text = totalOrderPrice.ToString("C2");
        }

        private void CalculateProductTimesAmount()
        {
            foreach (var orderItem in orderList)
            {
                orderItem.TotalPrice = orderItem.Price * orderItem.Amount;
            }
        }

        private void CalculateTotalPrice(ref double totalOrderPrice)
        {
            foreach(var orderItem in orderList)
            {
                totalOrderPrice += orderItem.TotalPrice;
            }    
        }

        private void PrintDocument_ButtonClick(object sender, RoutedEventArgs e)
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
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, fs);
                        pdfDoc.Open();
                        pdfDoc.NewPage();

                        Paragraph pdfTitle = CreatePdfTtiel();
                        PdfPTable pdfPTable = CreatePdfTable();
                        Paragraph pdfFooter = CreatePdfFooter();

                        pdfDoc.Add(pdfTitle);
                        pdfDoc.Add(pdfPTable);
                        pdfDoc.Add(pdfFooter);

                        pdfDoc.Close();
                    }

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

        private Paragraph CreatePdfTtiel()
        {
            Paragraph pdfTitle = new Paragraph("Zamówienie", new Font(Font.FontFamily.TIMES_ROMAN, 18, Font.BOLD));
            pdfTitle.Alignment = Element.ALIGN_CENTER;
            pdfTitle.SpacingAfter = 10f;
            pdfTitle.SetLeading(0, 1.2f);

            return pdfTitle;
        }

        private PdfPTable CreatePdfTable()
        {
            PdfPTable pdfTable = new PdfPTable(orderSummaryDataGrid.Columns.Count);

            foreach (DataGridColumn column in orderSummaryDataGrid.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(column.Header.ToString()));
                pdfTable.AddCell(cell);
            }

            foreach (var item in orderSummaryDataGrid.Items)
            {
                foreach (DataGridColumn column in orderSummaryDataGrid.Columns)
                {
                    string cellValue = (column.GetCellContent(item) as TextBlock)?.Text;
                    PdfPCell cell = new PdfPCell(new Phrase(cellValue ?? ""));
                    pdfTable.AddCell(cell);
                }
            }

            return pdfTable;
        }

        private Paragraph CreatePdfFooter()
        {
            string footer = $"Podsumowanie ceny zamówienia: {totalOrderPrice} zl";
            Paragraph pdfFooter = new Paragraph(footer);
            pdfFooter.Alignment = Element.ALIGN_CENTER;
            pdfFooter.SpacingBefore = 10f;
            pdfFooter.SetLeading(0, 1.2f);

            return pdfFooter;
        }

        private void CloseWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }
    }
}

