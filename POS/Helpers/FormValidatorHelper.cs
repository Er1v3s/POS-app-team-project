using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.IdentityModel.Tokens;

namespace POS.Helpers
{
    public class FormValidatorHelper
    {
        public static string ValidateString(string stringToValidate)
        {
            if (stringToValidate.IsNullOrEmpty())
                throw new Exception("Niekompletne dane");
            
            return stringToValidate;
        }

        public static string ValidateEmailAddress(string email)
        {
            var emailValidated = ValidateString(email);

            if (!IsEmailValid(emailValidated))
                throw new Exception("Niepoprawny adres email");

            return emailValidated;
        }

        public static string ValidatePhoneNumber(string phoneNumber)
        {
            var phoneNumberValidated = ValidateString(phoneNumber);

            if (!IsPhoneNumberValid(phoneNumberValidated))
                throw new Exception("Niepoprawny numer telefonu");

            return phoneNumberValidated;
        }

        public static void ValidateTextBox(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.BorderThickness = new Thickness(2);

            textBox.BorderBrush = textBox.Text.Length < 1 ?
                new SolidColorBrush(Color.FromRgb(174, 75, 89)) :
                new SolidColorBrush(Color.FromRgb(55, 154, 140));
        }

        public static void ValidateEmailAddress(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.BorderThickness = new Thickness(2);

            if (textBox != null || textBox.Text.Length >= 1)
            {
                textBox.BorderBrush = IsEmailValid(textBox.Text) ?
                    new SolidColorBrush(Color.FromRgb(55, 154, 140)) :
                    new SolidColorBrush(Color.FromRgb(174, 75, 89));
            }
        }

        public static void ValidatePhoneNumber(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.BorderThickness = new Thickness(2);

            textBox.BorderBrush = IsPhoneNumberValid(textBox.Text) ?
                new SolidColorBrush(Color.FromRgb(55, 154, 140)) :
                new SolidColorBrush(Color.FromRgb(174, 75, 89));
        }

        public static void ValidateTaxIdentificationNumber(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.BorderThickness = new Thickness(2);

            textBox.BorderBrush = textBox.Text.Length != 10 ?
                new SolidColorBrush(Color.FromRgb(174, 75, 89)) :
                new SolidColorBrush(Color.FromRgb(55, 154, 140));
        }

        private static bool IsEmailValid(string email)
        {
            string regexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            Regex regex = new Regex(regexPattern);
            return regex.IsMatch(email);
        }

        private static bool IsPhoneNumberValid(string phoneNumber)
        {
            string regexPattern = @"^[1-9]\d{8}$";

            Regex regex = new Regex(regexPattern);
            return regex.IsMatch(phoneNumber);
        }
    }
}
