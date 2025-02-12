using System.Windows;
using System.Windows.Controls;

namespace POS.Helpers
{
    public class PlaceholderTextBoxHelper
    {
        public static void SetPlaceholderOnFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Text.Length > 0 && textBox.Text == textBox.Tag.ToString())
            {
                textBox.Text = "";
            }
        }

        public static void SetPlaceholderOnLostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
                textBox.Text = textBox.Tag.ToString();
        }
    }
}
