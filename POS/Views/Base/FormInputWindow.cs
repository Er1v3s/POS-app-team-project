using System.Windows;
using POS.Validators;

namespace POS.Views.Base
{
    public abstract class FormInputWindow : WindowBase
    {
        protected void FormInput_LostFocus(object sender, RoutedEventArgs e)
        {
            FormValidator.ValidateTextBox(sender, e);
        }

        protected void EmailFormInput_LostFocus(object sender, RoutedEventArgs e)
        {
            FormValidator.ValidateEmailAddress(sender, e);
        }

        protected void PhoneNumberFormInput_LostFocus(object sender, RoutedEventArgs e)
        {
            FormValidator.ValidatePhoneNumber(sender, e);
        }

        protected void TaxIdentificationNumberFormInput_LostFocus(object sender, RoutedEventArgs e)
        {
            FormValidator.ValidateTaxIdentificationNumber(sender, e);
        }
    }
}
