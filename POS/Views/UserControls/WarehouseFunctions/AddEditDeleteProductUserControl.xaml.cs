using System.ComponentModel;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.WarehouseFunctions;

namespace POS.Views.UserControls.WarehouseFunctions
{
    /// <summary>
    /// Interaction logic for AddEditDeleteProductUserControl.xaml
    /// </summary>
    public partial class AddEditDeleteProductUserControl : UserControl
    {
        public AddEditDeleteProductUserControl()
        {
            InitializeComponent();

            // Designer check
            if (DesignerProperties.GetIsInDesignMode(this))
                DataContext = new AddEditDeleteProductViewModel();
            else
                DataContext = App.ServiceProvider.GetRequiredService<AddEditDeleteProductViewModel>();
        }
    }
}
