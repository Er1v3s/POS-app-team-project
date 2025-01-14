using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using POS.Models.Orders;

namespace POS.Views.Windows.SalesPanel
{
    /// <summary>
    /// Logika interakcji dla klasy GenerateBill.xaml
    /// </summary>
    public partial class OrderSummaryWindow : Window
    {
        private List<OrderItemDto> orderList;
        double totalOrderPrice = 0;

        public OrderSummaryWindow(List<OrderItemDto> orderList)
        {
            InitializeComponent();

            this.orderList = orderList;

            CalculateProductTimesAmount();
            CalculateTotalPrice(ref totalOrderPrice);

            orderSummaryDataGrid.ItemsSource = this.orderList;
            totalPriceTextBlock.Text = totalOrderPrice.ToString("C2");
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void CloseWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CalculateProductTimesAmount()
        {
            foreach (var orderItem in orderList)
            {
                //orderItem.TotalPrice = orderItem.Price * orderItem.Amount;
            }
        }

        private void CalculateTotalPrice(ref double totalOrderPrice)
        {
            foreach (var orderItem in orderList)
            {
                //totalOrderPrice += orderItem.TotalPrice;
            }
        }

        private async void PrintDocument_ButtonClick(object sender, RoutedEventArgs e)
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
                    await using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, fs);
                        pdfDoc.Open();
                        pdfDoc.NewPage();

                        Paragraph pdfDateTime = CreateDateTime();
                        Paragraph pdfTitle = CreatePdfTitle();
                        PdfPTable pdfPTable = CreatePdfTable();
                        PdfPTable pdfSummary = CreateOrderSummary();

                        pdfDoc.Add(pdfDateTime);
                        pdfDoc.Add(pdfTitle);
                        pdfDoc.Add(spacer);

                        if(InvoiceWindow.InvoiceCustomerDataObject != null)
                        {
                            Paragraph pdfInvoiceInfo = CreateInvoiceInfo();
                            PdfPTable pdfInvoiceData = CreateInvoiceClientInfo();

                            pdfDoc.Add(pdfInvoiceInfo);
                            pdfDoc.Add(pdfInvoiceData);
                        }

                        pdfDoc.Add(spacer);
                        pdfDoc.Add(pdfPTable);
                        pdfDoc.Add(spacer);
                        pdfDoc.Add(pdfSummary);

                        pdfDoc.Close();
                    }

                    DialogResult = true;
                    this.Close();
                    MessageBox.Show("Rachunek został wygenrowany.");
                }
                else
                {
                    MessageBox.Show("Anulowano generowanie rachunku.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas generowania rachunku: " + ex.Message);
            }
        }

        private Paragraph CreateDateTime()
        {
            Paragraph dateTime = new Paragraph(DateTime.Now.ToString());
            dateTime.Alignment = Element.ALIGN_RIGHT;

            return dateTime;
        }

        private Paragraph CreatePdfTitle()
        {
            Paragraph pdfTitle = new Paragraph("Zamówienie", new Font(Font.FontFamily.TIMES_ROMAN, 18, Font.BOLD));
            pdfTitle.Alignment = Element.ALIGN_CENTER;
            pdfTitle.SpacingAfter = 10f;
            pdfTitle.SetLeading(0, 1.2f);

            return pdfTitle;
        }

        private Paragraph CreateInvoiceInfo()
        {
            Paragraph pdfInvoiceInfo = new Paragraph("Faktura VAT", new Font(Font.FontFamily.TIMES_ROMAN, 18, Font.BOLD));
            pdfInvoiceInfo.Alignment = Element.ALIGN_LEFT;
            pdfInvoiceInfo.SpacingAfter = 10f;
            pdfInvoiceInfo.SetLeading(0, 1.2f);

            return pdfInvoiceInfo;
        }

        private PdfPTable CreateInvoiceClientInfo() 
        {
            PdfPTable clientInfoTable = new PdfPTable(new[] { .75f, 2f })
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                WidthPercentage = 75,
                DefaultCell = { MinimumHeight = 22f }
            };

            clientInfoTable.AddCell("NIP");
            clientInfoTable.AddCell(InvoiceWindow.InvoiceCustomerDataObject.TaxIdentificationNumber.ToString());
            clientInfoTable.AddCell("Nazwa");
            clientInfoTable.AddCell(InvoiceWindow.InvoiceCustomerDataObject.CustomerName);
            clientInfoTable.AddCell("Adres");
            clientInfoTable.AddCell(InvoiceWindow.InvoiceCustomerDataObject.CustomerAddress);

            return clientInfoTable;
        }

        private PdfPTable CreatePdfTable()
        {
            PdfPTable pdfTable = new PdfPTable(orderSummaryDataGrid.Columns.Count)
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                WidthPercentage = 100,
                DefaultCell = { MinimumHeight = 22f },
            };

            foreach (DataGridColumn column in orderSummaryDataGrid.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(column.Header.ToString()))
                {
                    FixedHeight = 22f
                };
                pdfTable.AddCell(cell);
            }

            foreach (var item in orderSummaryDataGrid.Items)
            {
                foreach (DataGridColumn column in orderSummaryDataGrid.Columns)
                {
                    string cellValue = (column.GetCellContent(item) as TextBlock)?.Text;
                    PdfPCell cell = new PdfPCell(new Phrase(cellValue ?? ""))
                    {
                        FixedHeight = 22f
                    };
                    pdfTable.AddCell(cell);
                }
            }

            return pdfTable;
        }

        private PdfPTable CreateOrderSummary()
        {
            PdfPTable pdfTable = new PdfPTable(new[] { .75f, 2f })
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                WidthPercentage = 100,
                DefaultCell = { MinimumHeight = 22f },
                
            };

            pdfTable.AddCell("SUMA");
            pdfTable.AddCell($"{totalOrderPrice} zl ");

            return pdfTable;
        }

        private Paragraph spacer = new Paragraph("")
        {
            SpacingBefore = 10f,
            SpacingAfter = 10f,
        };
    }
}

