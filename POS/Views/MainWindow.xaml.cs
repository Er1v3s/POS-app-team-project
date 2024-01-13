using POS.Models;
using POS.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace POS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private bool alertDisplayed = false;
        private bool expirationAlertDisplayed = false;
        public MainWindow()
        {
            InitializeComponent();
            StartTimer();
        }

        private void MoveToSalesPanel_ButtonClick(object sender, RoutedEventArgs e)
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

        private void TurnOffApplication_ButtonClick(object sender, RoutedEventArgs e)
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

        private void ShowLoginPanel(string uri)
        {
            LoginPanel loginPanel = new LoginPanel(uri);
            loginPanel.ShowDialog();
        }

        private void ShowLoginPanelAndChangeSource(string uri)
        {
            LoginPanel loginPanel = new LoginPanel(uri);
            loginPanel.ShowDialog();

            if(loginPanel.isLoginValid)
            {
                ChangeFrameSource(uri);
            } 
        }

        private void ChangeSource_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string uri)
            {
                if (uri == "./WorkTimeSummaryControl.xaml" || uri == "./RunningOutOfIngredients.xaml" || uri == "./ReportsAndAnalysis.xaml")
                {
                    ChangeFrameSource(uri);
                }
                else if (uri == "./AdministratorFuncions.xaml")
                {
                    ShowLoginPanelAndChangeSource(uri);
                }
                else
                {
                    ShowLoginPanel(uri);
                }
            }
        }

        private void StartTimer() 
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;
            timer.Start();

            // Initialize first date render
            UpdateDateTime(); 
        }
        private void TimerTick(object sender, EventArgs e)
        {
            UpdateDateTime();
            if (!alertDisplayed)
            {
                CheckIngredientLevels();
            }
            if (!expirationAlertDisplayed)
            {
                CheckIngredientExpiration();
            }
        }

        private void CheckIngredientLevels()
        {
            using (var dbContext = new AppDbContext())
            {
                var lowIngredients = dbContext.Ingredients
                    .Where(i => i.Stock.HasValue && i.Stock.Value < i.Safety_stock)
                    .ToList();

                if (lowIngredients.Any())
                {
                    string alertMessage = "Uwaga! Niektóre z poniższych składników się kończą:\n";
                    foreach (var ingredient in lowIngredients)
                    {
                        alertMessage += $"{ingredient.Name}\n";
                    }

                    MessageBoxButton button = MessageBoxButton.OKCancel;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBoxResult result = MessageBox.Show(alertMessage, "Alert o kończących się składnikach", button, icon);

                    if (result == MessageBoxResult.OK)
                    {
                        RunningOutOfIngredients runningOutOfIngredients = new RunningOutOfIngredients();
                        runningOutOfIngredients.ShowWindow();
                    }
                    alertDisplayed = true;

                    Task.Delay(TimeSpan.FromMinutes(3)).ContinueWith(_ => alertDisplayed = false);
                }
            }
        }

        private void CheckIngredientExpiration()
        {
            using (var dbContext = new AppDbContext())
            {
                var expiringIngredients = dbContext.Ingredients
                .AsEnumerable()
                .Where(i => i.Expiration_date != null && DateTime.ParseExact(i.Expiration_date, "yyyy-MM-dd", CultureInfo.InvariantCulture) <= DateTime.Now.AddDays(7))
                .ToList();


                if (expiringIngredients.Any())
                {
                    string alertMessage = "Uwaga! Niektóre ze składników tracą na ważności:\n";
                    foreach (var ingredient in expiringIngredients)
                    {
                        alertMessage += $"{ingredient.Name} (Data ważności: {ingredient.Expiration_date})\n";
                    }

                    MessageBoxButton button = MessageBoxButton.OKCancel;
                    MessageBoxImage icon = MessageBoxImage.Warning;

                    MessageBoxResult result = MessageBox.Show(alertMessage, "Alert o przeterminowaniu składników", button, icon);

                    if (result == MessageBoxResult.OK)
                    {
                        CreateDelivery createDelivery = new CreateDelivery();
                        createDelivery.Show();
                    }
                    expirationAlertDisplayed = true;

                    Task.Delay(TimeSpan.FromMinutes(3)).ContinueWith(_ => expirationAlertDisplayed = false);
                }
            }
        }

        private void UpdateDateTime()
        {
            dateTextBlock.Text = DateTime.Now.ToString("dd.MM.yyyy");
            timeTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
