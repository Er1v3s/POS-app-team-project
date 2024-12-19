using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.AdminFunctionsPanel;

namespace POS.Views.AdminFunctionsPanel
{
    /// <summary>
    /// Logika interakcji dla klasy AdministratorFuncions.xaml
    /// </summary>
    public partial class AdministratorFunctions : UserControl
    {
        public AdministratorFunctions()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<AdministratorFunctionsViewModel>();
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                row.IsSelected = !row.IsSelected;
                e.Handled = true;
            }
        }
    }
}