using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.WarehouseFunctions;

namespace POS.Views.Windows.WarehouseFunctions
{
    /// <summary>
    /// Logika interakcji dla klasy StockManagment.xaml
    /// </summary>
    public partial class StockManagementWindow : Window
    {
        public StockManagementWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<StockManagementViewModel>();

            var viewModel = (StockManagementViewModel)DataContext;
            viewModel.CloseWindowBaseAction = Close;
        }
    }
}
