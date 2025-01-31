using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.WarehouseFunctions;

namespace POS.Views.UserControls.WarehouseFunctions
{
    /// <summary>
    /// Interaction logic for EditProductRecipeUserControl.xaml
    /// </summary>
    public partial class EditProductRecipeUserControl : UserControl
    {
        public EditProductRecipeUserControl()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<EditProductRecipeViewModel>();
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                row.IsSelected = true;
                e.Handled = true;
            }
        }
    }
}
