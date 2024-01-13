﻿using POS.Models;
using POS.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

    }
}
