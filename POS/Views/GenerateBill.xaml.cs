using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using POS.Models;
using POS.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy GenerateBill.xaml
    /// </summary>
    public partial class GenerateBill
    {
        private ObservableCollection<OrderItem> orderList;

        public GenerateBill(ObservableCollection<OrderItem> orderList)
        {
            InitializeComponent();

            this.orderList = orderList;
            orderSummaryDataGrid.ItemsSource = this.orderList;

            double totalSum = CalculateTotalSum();
            totalPriceTextBlock.Text = totalSum.ToString("C");
        }

        private double CalculateTotalSum()
        {
            double totalSum = 0;
            foreach (var orderItem in orderList)
            {
                totalSum += orderItem.TotalPrice;
            }
            return Math.Round(totalSum, 2);
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
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, new FileStream(filePath, FileMode.Create));

                    pdfDoc.Open();
                    pdfDoc.NewPage();

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

                    Paragraph pdfTitle = new Paragraph("Zamówienie", new Font(Font.FontFamily.TIMES_ROMAN, 18, Font.BOLD));
                    pdfTitle.Alignment = Element.ALIGN_CENTER; 
                    pdfTitle.SpacingAfter = 10f; 
                    pdfTitle.SetLeading(0, 1.2f);

                    string footer = "Podsumowanie ceny zamówienia: " + CalculateTotalSum().ToString() + "zl";
                    Paragraph pdfFooter = new Paragraph(footer);
                    pdfFooter.Alignment = Element.ALIGN_CENTER; 
                    pdfFooter.SpacingBefore = 10f; 
                    pdfFooter.SetLeading(0, 1.2f); 

                    pdfDoc.Add(pdfTitle);
                    pdfDoc.Add(pdfTable);
                    pdfDoc.Add(pdfFooter);

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

        private void CloseWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }
    }
}

