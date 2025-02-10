using System.Windows;

namespace POS.Exceptions
{
    public static class ExceptionsHandler
    {
        public static void ShowErrorMessage(string message)
        {
            MessageBox.Show($"Wystąpił nieoczekiwany błąd: {message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
