using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.WarehouseFunctions;
using System.Windows;

namespace POS.Views.Windows.WarehouseFunctions
{
    /// <summary>
    /// Interaction logic for ProductManagementWindow.xaml
    /// </summary>
    public partial class ProductManagementWindow : Window
    {
        public ProductManagementWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<ProductManagementViewModel>();

            var viewModel = (ProductManagementViewModel)DataContext;
            viewModel.CloseWindowBaseAction = Close;
        }
    }
}
