using POS.Helpers;
using System.Windows;

namespace POS.Utilities
{
    public abstract class FormInputWindow : Window
    {
        protected FormInputWindow()
        {
            MouseLeftButtonDown += (sender, e) => DragMove();
        }

        protected void FormInput_LostFocus(object sender, RoutedEventArgs e)
        {
            FormValidatorHelper.ValidateTextBox(sender, e);
        }

        protected void EmailFormInput_LostFocus(object sender, RoutedEventArgs e)
        {
            FormValidatorHelper.ValidateEmailAddress(sender, e);
        }

        protected void PhoneNumberFormInput_LostFocus(object sender, RoutedEventArgs e)
        {
            FormValidatorHelper.ValidatePhoneNumber(sender, e);
        }
    }
}
