using System.Windows;

namespace POS.Views
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

        private void CloseWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
