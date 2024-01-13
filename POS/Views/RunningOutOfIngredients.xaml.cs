using POS.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy RunningOutOfIngredients.xaml
    /// </summary>
    public partial class RunningOutOfIngredients :UserControl
    {
        public RunningOutOfIngredients()
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
            LoginPanel loginPanel = new LoginPanel();
            loginPanel.ShowDialog();

            if (loginPanel.isLoginValid)
            {
                int employeeId = loginPanel.employeeId;
                StockManagment stockManagment = new StockManagment(employeeId);
                stockManagment.Show();
            }
        }

        private void OpenCreateDeliveryWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            LoginPanel loginPanel = new LoginPanel();
            loginPanel.ShowDialog();

            if (loginPanel.isLoginValid)
            {
                int employeeId = loginPanel.employeeId;
                CreateDelivery createDelivery = new CreateDelivery(employeeId);
                createDelivery.Show();
            }
        }

        private void LoadRunningOutOfIngredients()
        {
            try
            {
                using (var dbContext = new AppDbContext())
                {
                    var runningOutOfIngredients = dbContext.Ingredients
                        .Where(ingredient => ingredient.Stock < ingredient.Safety_stock)
                        .ToList();

                    runningOutOfIngredientsDataGrid.ItemsSource = runningOutOfIngredients;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas wczytywania składników: " + ex.Message);
            }
        }

        public void ShowWindow()
        {
            // Otwieranie kontrolki (okna) RunningOutOfIngredients
            var window = new Window();
            window.Content = this;
            window.ShowDialog();
        }
    }
}
