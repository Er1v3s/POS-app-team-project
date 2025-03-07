using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.WarehouseFunctions;
using System.Windows.Controls;

namespace POS.Views.UserControls.WarehouseFunctions
{
    /// <summary>
    /// Interaction logic for CreateDeliveryUserControl.xaml
    /// </summary>
    public partial class CreateDeliveryUserControl : UserControl
    {
        public CreateDeliveryUserControl()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<CreateDeliveryViewModel>();
        }
    }
}
