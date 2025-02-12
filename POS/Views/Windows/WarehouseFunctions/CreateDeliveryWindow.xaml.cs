using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.WarehouseFunctions;

namespace POS.Views.Windows.WarehouseFunctions
{
    /// <summary>
    /// Logika interakcji dla klasy CreateDelivery.xaml
    /// </summary>
    public partial class CreateDeliveryWindow : Window
    {
        public CreateDeliveryWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<CreateDeliveryViewModel>();

            var viewModel = (CreateDeliveryViewModel)DataContext;
            viewModel.CloseWindowBaseAction = Close;
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
