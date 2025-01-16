using iTextSharp.text.pdf;
using iTextSharp.text;
using POS.Views.Windows.SalesPanel;
using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using POS.Models.Orders;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.SalesPanel
{
    public class OrderSummaryViewModel : ViewModelBase
    {
        private bool dialogResult;

        private List<OrderItemDto> orderList;
        private double amountToPayForOrder;

        private readonly Paragraph lineSpacer = new("")
        {
            SpacingBefore = 10f,
            SpacingAfter = 10f,
        };

        public bool DialogResult
        {
            get => dialogResult;
            set => SetField(ref dialogResult, value);
        }

        public List<OrderItemDto> OrderList
        {
            get => orderList;
            set => SetField(ref orderList, value);
        }

        public double AmountToPayForOrder
        {
            get => amountToPayForOrder;
            set => SetField(ref amountToPayForOrder, Math.Round(value, 2));
        }

        public Action CloseWindowAction;
        public ICommand FinishOrderCommand { get; }

        public OrderSummaryViewModel()
        {
            FinishOrderCommand = new RelayCommandAsync(FinishOrder);
        }

        private async Task FinishOrder()
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
                    await CreatePdfDocument(filePath);

                    DialogResult = true;
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

            CloseWindowAction.Invoke();
        }

        private Paragraph CreateDateTime()
        {
            Paragraph dateTime = new Paragraph(DateTime.Now.ToString());
            dateTime.Alignment = Element.ALIGN_RIGHT;

            return dateTime;
        }

        private async Task CreatePdfDocument(string filePath)
        {
            Document pdfDoc = new Document(PageSize.A4);

            await using FileStream fs = new FileStream(filePath, FileMode.Create);
            PdfWriter.GetInstance(pdfDoc, fs);
            pdfDoc.Open();
            pdfDoc.NewPage();

            Paragraph pdfDateTime = CreateDateTime();
            Paragraph pdfTitle = CreatePdfTitle();
            PdfPTable pdfPTable = CreatePdfTable();
            PdfPTable pdfSummary = CreateOrderSummary();

            pdfDoc.Add(pdfDateTime);
            pdfDoc.Add(pdfTitle);
            pdfDoc.Add(lineSpacer);

            if (InvoiceWindow.InvoiceCustomerDataObject != null)
            {
                Paragraph pdfInvoiceInfo = CreateInvoiceInfo();
                PdfPTable pdfInvoiceData = CreateInvoiceClientInfo();

                pdfDoc.Add(pdfInvoiceInfo);
                pdfDoc.Add(pdfInvoiceData);
            }

            pdfDoc.Add(lineSpacer);
            pdfDoc.Add(pdfPTable);
            pdfDoc.Add(lineSpacer);
            pdfDoc.Add(pdfSummary);

            pdfDoc.Close();
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
            PdfPTable pdfTable = new PdfPTable(5)
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                WidthPercentage = 100,
                DefaultCell = { MinimumHeight = 22f },
            };

            AddHeaderRow(pdfTable);

            var rows = orderList.Select((order, index) => new[]
            {
                CreateCell($"{index + 1}"),
                CreateCell(order.ProductName),
                CreateCell($"{order.Amount}"),
                CreateCell($"{order.Price} PLN"),
                CreateCell($"{order.TotalPrice} PLN")
            });

            foreach (var row in rows)
            {
                foreach (var cell in row)
                    pdfTable.AddCell(cell);
            }

            return pdfTable;
        }

        private void AddHeaderRow(PdfPTable pdfTable)
        {
            var headers = new[]
            {
                "Lp.", "Nazwa", "Ilosc", "Cena", "Razem"
            };

            foreach (var header in headers)
            {
                pdfTable.AddCell(new PdfPCell(new Phrase(header))
                {
                    FixedHeight = 25f,
                    BackgroundColor = new BaseColor(220, 220, 220),
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });
            }
        }

        private PdfPCell CreateCell(string text)
        {
            return new PdfPCell(new Phrase(text))
            {
                FixedHeight = 22f,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
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
            pdfTable.AddCell($"{amountToPayForOrder} PLN ");

            return pdfTable;
        }
    }
}
