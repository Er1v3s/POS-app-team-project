using POS.Models;
using POS.Views;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace POS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;

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

        private async void StartTimer() 
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;
            timer.Start();

            await UpdateDateTimeAsync();

            ScheduleDelayedExecution(CheckIngredientLevels, TimeSpan.FromSeconds(60));
            ScheduleDelayedExecution(CheckIngredientExpiration, TimeSpan.FromSeconds(60));
        }

        private async void TimerTick(object sender, EventArgs e)
        {
            await UpdateDateTimeAsync();
        }

        private async Task UpdateDateTimeAsync()
        {
            await Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    dateTextBlock.Text = DateTime.Now.ToString("dd.MM.yyyy");
                    timeTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");
                });
            });
        }

        private void ScheduleDelayedExecution(Action action, TimeSpan delay)
        {
            Task.Delay(delay).ContinueWith(_ => Dispatcher.Invoke(action));
        }

        private void CheckIngredientLevels()
        {
            using (var dbContext = new AppDbContext())
            {
                var lowIngredients = dbContext.Ingredients
                    .Where(i => i.Stock.HasValue && i.Stock.Value < i.SafetyStock)
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
                }
            }
        }

        private void CheckIngredientExpiration()
        {
            using (var dbContext = new AppDbContext())
            {
                var expiringIngredients = dbContext.Ingredients
                .AsEnumerable()
                .Where(i => i.ExpirationDate != null && DateTime.ParseExact(i.ExpirationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) <= DateTime.Now.AddDays(7))
                .ToList();


                if (expiringIngredients.Any())
                {
                    string alertMessage = "Uwaga! Niektóre ze składników tracą na ważności:\n";
                    foreach (var ingredient in expiringIngredients)
                    {
                        alertMessage += $"{ingredient.Name} (Data ważności: {ingredient.ExpirationDate})\n";
                    }

                    MessageBoxButton button = MessageBoxButton.OKCancel;
                    MessageBoxImage icon = MessageBoxImage.Warning;

                    MessageBoxResult result = MessageBox.Show(alertMessage, "Alert o przeterminowaniu składników", button, icon);

                    if (result == MessageBoxResult.OK)
                    {
                        CreateDelivery createDelivery = new CreateDelivery();
                        createDelivery.Show();
                    }
                }
            }
        }
    }
}
