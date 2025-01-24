using System.Windows;
using POS.Helpers;

namespace POS.Views.Base
{
    public abstract class FormInputWindow : WindowBase
    {
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

        protected void TaxIdentificationNumberFormInput_LostFocus(object sender, RoutedEventArgs e)
        {
            FormValidatorHelper.ValidateTaxIdentificationNumber(sender, e);
        }
    }
}
