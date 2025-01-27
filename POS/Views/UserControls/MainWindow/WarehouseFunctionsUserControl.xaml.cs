using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.WarehouseFunctions;

namespace POS.Views.UserControls.MainWindow
{
    /// <summary>
    /// Logika interakcji dla klasy RunningOutOfIngredients.xaml
    /// </summary>
    public partial class WarehouseFunctionsUserControl : UserControl
    {
        public WarehouseFunctionsUserControl()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<WarehouseFunctionsViewModel>();


            var viewModel = (WarehouseFunctionsViewModel)DataContext;
            viewModel.LoadRunningOutOfIngredientsCommand.Execute(null);


            //LoadRunningOutOfIngredients();
        }

        //private void OpenStockManagmentWindow_ButtonClick(object sender, RoutedEventArgs e)
        //{
            //var loginPanel = new Windows.LoginPanelWindow();
            //loginPanel.ShowDialog();

            //if (loginPanel.isLoginValid)
            //{
            //    int employeeId = loginPanel.employeeId;
            //    StockManagement stockManagment = new StockManagement(employeeId);
            //    stockManagment.Show();
            //}
        //}

        private void OpenCreateDeliveryWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            var loginPanel = new Windows.LoginPanelWindow();
            loginPanel.ShowDialog();

            //if (loginPanel.isLoginValid)
            //{
            //    int employeeId = loginPanel.employeeId;
            //    CreateDelivery createDelivery = new CreateDelivery(employeeId);
            //    createDelivery.Show();
            //}
        }
    }
}
