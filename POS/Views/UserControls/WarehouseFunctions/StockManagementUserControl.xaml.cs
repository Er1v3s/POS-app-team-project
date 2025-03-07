using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using System.Windows.Input;
using POS.ViewModels.WarehouseFunctions;

namespace POS.Views.UserControls.WarehouseFunctions
{
    /// <summary>
    /// Interaction logic for StockManagementUserControl.xaml
    /// </summary>
    public partial class StockManagementUserControl : UserControl
    {
        public StockManagementUserControl()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<StockManagementViewModel>();
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
