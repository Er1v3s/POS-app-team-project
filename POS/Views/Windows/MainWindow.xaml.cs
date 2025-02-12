using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.MainWindow;

namespace POS.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<MainWindowViewModel>();

            var viewModel = (MainWindowViewModel)DataContext;
            viewModel.TurnOffApplicationAction = Application.Current.Shutdown;
            viewModel.CloseWindowBaseAction = Close;
        }

        //private void ScheduleDelayedExecution(Action action, TimeSpan delay)
        //{
        //    Task.Delay(delay).ContinueWith(_ => Dispatcher.Invoke(action));
        //}

        //private void CheckIngredientLevels()
        //{
        //    using (var dbContext = new AppDbContext())
        //    {
        //        var lowIngredients = dbContext.Ingredients
        //            .Where(i => i.Stock.HasValue && i.Stock.Value < i.SafetyStock)
        //            .ToList();

        //        if (lowIngredients.Any())
        //        {
        //            string alertMessage = "Uwaga! Niektóre z poniższych składników się kończą:\n";
        //            foreach (var ingredient in lowIngredients)
        //            {
        //                alertMessage += $"{ingredient.Name}\n";
        //            }

        //            MessageBoxButton button = MessageBoxButton.OKCancel;
        //            MessageBoxImage icon = MessageBoxImage.Warning;
        //            MessageBoxResult result = MessageBox.Show(alertMessage, "Alert o kończących się składnikach", button, icon);

        //            if (result == MessageBoxResult.OK)
        //            {
        //                RunningOutOfIngredients runningOutOfIngredients = new RunningOutOfIngredients();
        //                runningOutOfIngredients.ShowWindow();
        //            }
        //        }
        //    }
        //}

        //private void CheckIngredientExpiration()
        //{
        //    using (var dbContext = new AppDbContext())
        //    {
        //        var expiringIngredients = dbContext.Ingredients
        //        .AsEnumerable()
        //        .Where(i => i.ExpirationDate != null && DateTime.ParseExact(i.ExpirationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) <= DateTime.Now.AddDays(7))
        //        .ToList();


        //        if (expiringIngredients.Any())
        //        {
        //            string alertMessage = "Uwaga! Niektóre ze składników tracą na ważności:\n";
        //            foreach (var ingredient in expiringIngredients)
        //            {
        //                alertMessage += $"{ingredient.Name} (Data ważności: {ingredient.ExpirationDate})\n";
        //            }

        //            MessageBoxButton button = MessageBoxButton.OKCancel;
        //            MessageBoxImage icon = MessageBoxImage.Warning;

        //            MessageBoxResult result = MessageBox.Show(alertMessage, "Alert o przeterminowaniu składników", button, icon);

        //            if (result == MessageBoxResult.OK)
        //            {
        //                CreateDelivery createDelivery = new CreateDelivery();
        //                createDelivery.Show();
        //            }
        //        }
        //    }
        //}
    }
}
