using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.SalesPanel;
using POS.Views.Base;

namespace POS.Views.Windows.SalesPanel
{
    /// <summary>
    /// Logika interakcji dla klasy FinishedOrders.xaml
    /// </summary>
    public partial class FinishedOrdersWindow : WindowBase
    {
        public FinishedOrdersWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<FinishedOrderViewModel>();

            var viewModel = (FinishedOrderViewModel)DataContext;
            viewModel.CloseWindowBaseAction = Close;
        }
    }
}