using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using POS.Models.Invoices;
using POS.Models.Orders;

namespace POS.Services.SalesPanel
{
    public class OrderSummaryService
    {
        private readonly Paragraph lineSpacer = new("")
        {
            SpacingBefore = 10f,
            SpacingAfter = 10f,
        };

        public async Task<bool> GenerateBill(List<OrderItemDto> orderList, double amountToPayForOrder, int discount, InvoiceCustomerDataDto? invoiceCustomerData)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Pliki PDF (*.pdf)|*.pdf";
                saveFileDialog.FileName = "Podsumowanie_zamowienia.pdf";

                bool? result = saveFileDialog.ShowDialog();

                if (result == true)
                {
                    string filePath = saveFileDialog.FileName;
                    await CreatePdfDocument(filePath, orderList, amountToPayForOrder, discount, invoiceCustomerData);

                    MessageBox.Show("Rachunek został wygenrowany.");
                    return true;
                }

                MessageBox.Show("Anulowano generowanie rachunku.");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas generowania rachunku: " + ex.Message);
                return false;
            }
        }

        private Paragraph CreateDateTime()
        {
            Paragraph dateTime = new (DateTime.Now.ToString("G"))
            {
                Alignment = Element.ALIGN_RIGHT
            };

            return dateTime;
        }

        private async Task CreatePdfDocument(string filePath, List<OrderItemDto> orderList, double amountToPayForOrder, int discount, InvoiceCustomerDataDto? invoiceCustomerDataDto = null)
        {
            Document pdfDoc = new (PageSize.A4);

            await using FileStream fs = new FileStream(filePath, FileMode.Create);
            PdfWriter.GetInstance(pdfDoc, fs);
            pdfDoc.Open();
            pdfDoc.NewPage();

            var pdfDateTime = CreateDateTime();
            var pdfTitle = CreatePdfTitle();
            var pdfPTable = CreatePdfTable(orderList);
            var pdfDiscount = CreateDiscountRow(discount);
            var pdfSummary = CreateOrderSummary(amountToPayForOrder);

            pdfDoc.Add(pdfDateTime);
            pdfDoc.Add(pdfTitle);
            pdfDoc.Add(lineSpacer);

            if (invoiceCustomerDataDto != null)
            {
                var pdfInvoiceInfo = CreateInvoiceInfo();
                var pdfInvoiceData = CreateInvoiceClientInfo(invoiceCustomerDataDto);

                pdfDoc.Add(pdfInvoiceInfo);
                pdfDoc.Add(pdfInvoiceData);
            }

            pdfDoc.Add(lineSpacer);
            pdfDoc.Add(pdfPTable);
            pdfDoc.Add(lineSpacer);

            if (discount != 0)
            {
                pdfDoc.Add(lineSpacer);
                pdfDoc.Add(pdfDiscount);
            }

            pdfDoc.Add(pdfSummary);
            pdfDoc.Close();
        }

        private Paragraph CreatePdfTitle()
        {
            Paragraph pdfTitle = new ("Zamówienie", new Font(Font.FontFamily.TIMES_ROMAN, 18, Font.BOLD))
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 10f
            };
            pdfTitle.SetLeading(0, 1.2f);

            return pdfTitle;
        }

        private Paragraph CreateInvoiceInfo()
        {
            Paragraph pdfInvoiceInfo = new ("Faktura VAT", new Font(Font.FontFamily.TIMES_ROMAN, 18, Font.BOLD))
            {
                Alignment = Element.ALIGN_LEFT,
                SpacingAfter = 10f
            };
            pdfInvoiceInfo.SetLeading(0, 1.2f);

            return pdfInvoiceInfo;
        }

        private PdfPTable CreateInvoiceClientInfo(InvoiceCustomerDataDto invoiceCustomerDataDto)
        {
            PdfPTable clientInfoTable = new (new[] { .75f, 2f })
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                WidthPercentage = 75,
                DefaultCell = { MinimumHeight = 22f }
            };

            clientInfoTable.AddCell("NIP");
            clientInfoTable.AddCell(invoiceCustomerDataDto.TaxIdentificationNumber);
            clientInfoTable.AddCell("Nazwa");
            clientInfoTable.AddCell(invoiceCustomerDataDto.CustomerName);
            clientInfoTable.AddCell("Adres");
            clientInfoTable.AddCell(invoiceCustomerDataDto.CustomerAddress);

            return clientInfoTable;
        }

        private PdfPTable CreatePdfTable(List<OrderItemDto> orderList)
        {
            PdfPTable pdfTable = new (5)
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


        private PdfPTable CreateOrderSummary(double amountToPayForOrder)
        {
            PdfPTable pdfTable = new (new[] { .75f, 2f })
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                WidthPercentage = 100,
                DefaultCell = { MinimumHeight = 22f },
            };

            pdfTable.AddCell("SUMA");
            pdfTable.AddCell($"{amountToPayForOrder} PLN ");

            return pdfTable;
        }

        private PdfPTable CreateDiscountRow(int discount)
        {
            PdfPTable pdfTable = new (new[] { .75f, 2f })
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                WidthPercentage = 100,
                DefaultCell = { MinimumHeight = 22f },
            };

            pdfTable.AddCell("RABAT");
            pdfTable.AddCell($"-{discount} %");

            return pdfTable;
        }
    }
}
