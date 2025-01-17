using System.Windows;
using POS.Views.Windows.SalesPanel;

namespace POS.Services.SalesPanel
{
    public class DiscountService
    {
        public int SetDiscount()
        {
            var discountValue = GetDiscount();

            return discountValue;
        }

        private int GetDiscount()
        {
            var discountWindow = new DiscountWindow();
            discountWindow.ShowDialog();

            if (discountWindow.DialogResult == true)
            {
                if (discountWindow.radioButton10.IsChecked == true)
                    return 10;
                else if (discountWindow.radioButton15.IsChecked == true)
                    return 15;
                else
                    MessageBox.Show("Nie wybrano żadnego rabatu", "Ostrzeżenie", MessageBoxButton.OK);
            }

            return 0;
        }
    }
}
