using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.AdminFunctionsPanel;

namespace POS.Views.UserControls.MainWindow
{
    /// <summary>
    /// Logika interakcji dla klasy AdministratorFuncions.xaml
    /// </summary>
    public partial class AdminFunctionsUserControl : UserControl
    {
        public AdminFunctionsUserControl()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<AdminFunctionsViewModel>();
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