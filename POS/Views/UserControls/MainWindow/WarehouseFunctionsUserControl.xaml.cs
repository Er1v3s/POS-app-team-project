using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DataAccess;
using POS.Views.Windows;

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
            LoadRunningOutOfIngredients();
        }

        private void Refresh_ButtonClick(object sender, RoutedEventArgs e)
        {
            LoadRunningOutOfIngredients();
        }

        private void OpenStockManagmentWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            var loginPanel = new Windows.LoginPanelWindow();
            loginPanel.ShowDialog();

            //if (loginPanel.isLoginValid)
            //{
            //    int employeeId = loginPanel.employeeId;
            //    StockManagement stockManagment = new StockManagement(employeeId);
            //    stockManagment.Show();
            //}
        }

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

        private void LoadRunningOutOfIngredients()
        {
            try
            {
                using var dbContext = new AppDbContext();

                var runningOutOfIngredients = dbContext.Ingredients
                    .Where(ingredient => ingredient.Stock < ingredient.SafetyStock)
                    .ToList();

                runningOutOfIngredientsDataGrid.ItemsSource = runningOutOfIngredients;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas wczytywania składników: " + ex.Message);
            }
        }

        public void ShowWindow()
        {
            var window = new Window();
            window.Content = this;
            window.ShowDialog();
        }
    }
}
