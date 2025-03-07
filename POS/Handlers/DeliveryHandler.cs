using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using POS.Models.Warehouse;

namespace POS.Handlers
{
    public static class DeliveryHandler
    {
        private static readonly Paragraph lineSpacer = new("")
        {
            SpacingBefore = 10f,
            SpacingAfter = 10f,
        };

        public static async Task<bool> GenerateDeliveryDocument(List<DeliveryDto> deliveryItemList)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Pliki PDF (*.pdf)|*.pdf";
                saveFileDialog.FileName = $"Zamowienie-{DateTime.UtcNow:dd-MM-yyyy_HH-ss}.pdf";

                bool? result = saveFileDialog.ShowDialog();

                if (result == true)
                {
                    string filePath = saveFileDialog.FileName;
                    await CreatePdfDocument(filePath, deliveryItemList);

                    MessageBox.Show("Dokument dostawy został wygenrowany.");
                    return true;
                }

                MessageBox.Show("Anulowano generowanie dokumentu.");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas generowania dokumentu: " + ex.Message);
                return false;
            }
        }

        private static async Task CreatePdfDocument(string filePath, List<DeliveryDto> deliveryItemList)
        {
            Document pdfDoc = new(PageSize.A4);

            await using FileStream fs = new FileStream(filePath, FileMode.Create);
            PdfWriter.GetInstance(pdfDoc, fs);
            pdfDoc.Open();
            pdfDoc.NewPage();

            var pdfDateTime = CreateDateTime();
            var pdfTitle = CreatePdfTitle();
            var pdfPTable = CreatePdfTable(deliveryItemList);

            pdfDoc.Add(pdfDateTime);
            pdfDoc.Add(pdfTitle);
            pdfDoc.Add(lineSpacer);

            pdfDoc.Add(pdfPTable);
            pdfDoc.Add(lineSpacer);

            pdfDoc.Close();
        }

        private static Paragraph CreateDateTime()
        {
            Paragraph dateTime = new(DateTime.Now.ToString("G"))
            {
                Alignment = Element.ALIGN_RIGHT
            };

            return dateTime;
        }

        private static Paragraph CreatePdfTitle()
        {
            Paragraph pdfTitle = new("Zamówienie", new Font(Font.FontFamily.TIMES_ROMAN, 18, Font.BOLD))
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 10f
            };
            pdfTitle.SetLeading(0, 1.2f);

            return pdfTitle;
        }

        private static PdfPTable CreatePdfTable(List<DeliveryDto> deliveryItemList)
        {
            PdfPTable pdfTable = new(4)
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                WidthPercentage = 100,
                DefaultCell = { MinimumHeight = 22f },
            };

            AddHeaderRow(pdfTable);

            var rows = deliveryItemList.Select((item, index) => new[]
            {
                CreateCell($"{index + 1}"),
                CreateCell($"{item.Ingredient.Name}"),
                CreateCell($"{item.Ingredient.Package} - ({item.Ingredient.Unit}.)"),
                CreateCell($"{item.Quantity}")
            });

            foreach (var row in rows)
            {
                foreach (var cell in row)
                    pdfTable.AddCell(cell);
            }

            return pdfTable;
        }

        private static void AddHeaderRow(PdfPTable pdfTable)
        {
            var headers = new[]
            {
                "Lp.", "Nazwa produktu", "Opakowanie produktu", "Ilosc"
            };


            float[] columnWidths = new float[headers.Length];

            for (int i = 0; i < headers.Length; i++)
            {
                columnWidths[i] = headers[i].Length + 3;
            }

            pdfTable.SetWidths(columnWidths);

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

        private static PdfPCell CreateCell(string text)
        {
            return new PdfPCell(new Phrase(text))
            {
                FixedHeight = 22f,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
        }
    }
}
