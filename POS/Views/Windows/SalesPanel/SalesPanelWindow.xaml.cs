using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.SalesPanel;

namespace POS.Views.Windows.SalesPanel
{
    /// <summary>
    /// Logika interakcji dla klasy SalesPanel.xaml
    /// </summary>
    public partial class SalesPanelWindow : Window
    {
        public SalesPanelWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<SalesPanelViewModel>();

            var viewModel = (SalesPanelViewModel)DataContext;
            viewModel.CloseWindowBaseAction = Close;
        }
    }
}