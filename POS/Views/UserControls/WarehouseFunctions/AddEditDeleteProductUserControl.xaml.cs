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
            DataContext = App.ServiceProvider.GetRequiredService<AddEditDeleteProductViewModel>();
        }
    }
}
