using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace POS.Helpers
{
    class FormValidatorHelper
    {
        public static string ValidateTextBox(object sender)
        {
            TextBox textBox = sender as TextBox;
            textBox.BorderThickness = new Thickness(2);

            if (textBox != null)
            {
                if (textBox.Text.Length < 1)
                {
                    textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(174, 75, 89));
                    throw new Exception("Niepoprawna długość");
                }
                else
                {
                    textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(55, 154, 140));
                }
            }
            else
            {
                throw new Exception("Błąd podczas przetwarzania formularza");
            }

            return textBox.Text;
        }

        public static void ValidateTextBox(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.BorderThickness = new Thickness(2);

            if(textBox != null)
            {
                if (textBox.Text.Length < 1)
                {
                    textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(174, 75, 89));
                }
                else
                {
                    textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(55, 154, 140));
                }
            }
        }

        public static string ValidateComboBox(object sender)
        {
            ComboBox comboBox = (sender as ComboBox);
            comboBox.BorderThickness = new Thickness(2);

            if(comboBox.SelectedItem != null)
            {
                comboBox.BorderBrush = new SolidColorBrush(Color.FromRgb(55, 154, 140));
            }
            else
            {
                comboBox.BorderBrush = new SolidColorBrush(Color.FromRgb(174, 75, 89));
                throw new Exception("Nie wybrano żadnej wartości w jednym z pól");
            }

            return (comboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
        }

        public static string ValidateEmailAddress(object sender)
        {
            TextBox textBox = sender as TextBox;
            string textBoxValue = ValidateTextBox(textBox);

            if (textBoxValue != null) 
            {
                if(IsEmailValid(textBoxValue))
                {
                    return textBoxValue;
                }
                else
                {
                    throw new Exception("Niepoprawny adres email");
                }
            }

            return textBoxValue;
        }

        public static void ValidateEmailAddress(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.BorderThickness = new Thickness(2);

            if (textBox != null || textBox.Text.Length >= 1)
            {
                if (IsEmailValid(textBox.Text))
                {
                    textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(55, 154, 140));
                }
                else
                {
                    textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(174, 75, 89));
                }
            }
        }

        private static bool IsEmailValid(string email)
        {
            string regexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            Regex regex = new Regex(regexPattern);
            return regex.IsMatch(email);
        }
    }
}
