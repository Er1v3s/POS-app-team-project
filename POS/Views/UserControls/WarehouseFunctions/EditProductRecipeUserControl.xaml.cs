using System.ComponentModel;
using System.Windows.Controls;
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

            // Designer check
            if (DesignerProperties.GetIsInDesignMode(this))
                DataContext = new EditProductRecipeViewModel();
            else 
                DataContext = App.ServiceProvider.GetRequiredService<EditProductRecipeViewModel>();
        }
    }
}
