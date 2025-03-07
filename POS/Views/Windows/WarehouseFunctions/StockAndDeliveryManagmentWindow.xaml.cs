using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.WarehouseFunctions;

namespace POS.Views.Windows.WarehouseFunctions
{
    /// <summary>
    /// Logika interakcji dla klasy StockManagment.xaml
    /// </summary>
    public partial class StockAndDeliveryManagementWindow : Window
    {
        public StockAndDeliveryManagementWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<StockAndDeliveryManagementViewModel>();

            var viewModel = (StockAndDeliveryManagementViewModel)DataContext;
            viewModel.CloseWindowBaseAction = Close;
        }
    }
}
