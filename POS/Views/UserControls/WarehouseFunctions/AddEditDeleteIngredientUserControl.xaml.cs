using System.ComponentModel;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.WarehouseFunctions;

namespace POS.Views.UserControls.WarehouseFunctions
{
    /// <summary>
    /// Interaction logic for AddEditDeleteIngredientUserControl.xaml
    /// </summary>
    public partial class AddEditDeleteIngredientUserControl : UserControl
    {
        public AddEditDeleteIngredientUserControl()
        {
            InitializeComponent();

            // Designer check
            if (DesignerProperties.GetIsInDesignMode(this))
                DataContext = new AddEditDeleteIngredientViewModel();
            else
                DataContext = App.ServiceProvider.GetRequiredService<AddEditDeleteIngredientViewModel>();
        }
    }
}
