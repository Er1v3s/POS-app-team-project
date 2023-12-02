using POS.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
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

namespace POS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Move_To_Sales_Panel(object sender, RoutedEventArgs e)
        {
            LoginPanel loginPanel = new LoginPanel();
            loginPanel.ShowDialog();
            if (loginPanel.isLoginValid)
            {
                int employeeId = loginPanel.employeeId;
                SalesPanel salesPanel = new SalesPanel(employeeId);
                salesPanel.Show();
                this.Close();
            }
        }

        private void Turn_Off_Application(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ChangeFrameSource(string uri)
        {
            try
            {
                Uri newFrameSource = new Uri(uri, UriKind.RelativeOrAbsolute);
                frame.Source = newFrameSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}");
            }
        }

        private void showLoginPanel(string uri)
        {
            LoginPanel loginPanel = new LoginPanel(uri);
            loginPanel.ShowDialog();
        }

        private void NavigateButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string uri)
            {
                if (uri == "./WorkTimeSummaryControl.xaml")
                {
                    ChangeFrameSource(uri);
                }
                else
                {
                    showLoginPanel(uri);
                }
            }
        }
    }
}
