using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using POS.Services;
using POS.Services.Login;
using POS.ViewModels.MainWindow;
using POS.Views.RegisterSale;
using POS.Views.WarehouseFunctionsPanel;

namespace POS.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TimerService _timerService;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<MainWindowViewModel>();

            var viewModel = (MainWindowViewModel)DataContext;
            viewModel.TurnOffApplicationAction = Application.Current.Shutdown;

            _timerService = new TimerService(UpdateDateTime);
            _timerService.Start();
        }

        private void MoveToSalesPanel_ButtonClick(object sender, RoutedEventArgs e)
        {
            LoginManager.Instance.IsAuthenticationOnlyRequired = true;
            LoginManager.OpenLoginWindow();

            if (LoginManager.Instance.SuccessfullyLoggedIn && LoginManager.Instance.Employee!.IsUserLoggedIn)
            {
                LoginManager.Instance.IsAuthenticationOnlyRequired = false;
                LoginManager.Instance.SuccessfullyLoggedIn = false;

                SalesPanel salesPanel = new SalesPanel(LoginManager.Instance.Employee.EmployeeId);
                salesPanel.Show();
                Close();
            }
        }

        private void ChangeSource_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string uri)
            {
                if (uri == "./WorkTimeSummaryPanel/WorkTimeSummaryControl.xaml")
                    ChangeFrameSource(uri);
                else if (uri == "./AdminFunctionsPanel/AdministratorFunctions.xaml" || uri == "./WarehouseFunctionsPanel/RunningOutOfIngredients.xaml" || uri == "./ReportsAndAnalysisPanel/ReportsAndAnalysis.xaml")
                    CheckIfUserIsLoggedInAndChangeSource(uri);
                else
                    LoginManager.OpenLoginWindow();
            }
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

        private void CheckIfUserIsLoggedInAndChangeSource(string uri)
        {
            if (LoginManager.Instance.IsAnySessionActive)
                ChangeFrameSource(uri);
            else
                LoginManager.OpenLoginWindow();
        }

        private void UpdateDateTime(string date, string time)
        {
            if (dateTextBlock.Dispatcher.CheckAccess())
            {
                dateTextBlock.Text = date;
                timeTextBlock.Text = time;
            }
            else
            {
                dateTextBlock.Dispatcher.Invoke(() => UpdateDateTime(date, time));
            }
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
