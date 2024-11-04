using System.Windows;
using System.Windows.Input;

namespace POS.Views.RegisterSale
{
    /// <summary>
    /// Logika interakcji dla klasy DiscountWindow.xaml
    /// </summary>
    public partial class DiscountWindow : Window
    {
        public DiscountWindow()
        {
            InitializeComponent();
        }

        private void ApplyDiscount_ButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
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
    }
}
