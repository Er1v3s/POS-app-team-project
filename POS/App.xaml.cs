using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using POS.Factories;
using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;
using POS.Services;
using POS.Services.AdminFunctions;
using POS.Services.Login;
using POS.Services.ToDoList;
using POS.ViewModels.AdminFunctionsPanel;
using POS.ViewModels.MainWindow;
using POS.ViewModels.ReportsAndAnalysis;
using POS.ViewModels.ReportsAndAnalysis.ChartGenerators.PredictionChartGenerators;
using POS.ViewModels.ReportsAndAnalysis.ChartGenerators.ReportChartGenerators;
using POS.ViewModels.ReportsAndAnalysis.Factories;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;
using POS.ViewModels.ReportsAndAnalysis.PredictionGenerators;
using POS.ViewModels.ReportsAndAnalysis.ReportGenerators;
using POS.ViewModels.StartFinishWork;
using POS.ViewModels.ToDoList;
using POS.ViewModels.WorkTimeSummaryControl;

namespace POS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            InitializeComponent();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection servicesCollection)
        {
            servicesCollection.AddSingleton<AppDbContext>();

            #region MainWindow

            servicesCollection.AddTransient<TimeService>();
            servicesCollection.AddTransient<ViewFactory>();
            servicesCollection.AddTransient<NavigationService>();
            servicesCollection.AddTransient<MainWindowViewModel>();

            #endregion

            #region ReportsAndAnalysis

            servicesCollection.AddTransient<ReportsAndAnalysisViewModel>();

            // Reports
            servicesCollection.AddTransient<IReportGenerator<ProductSalesDto>, SalesReportGenerator>();
            servicesCollection.AddTransient<IReportGenerator<RevenueReportDto>, RevenueReportGenerator>();
            servicesCollection.AddTransient<IReportGenerator<OrderReportDto>, NumberOfOrdersGenerator>();
            servicesCollection.AddTransient<IReportGenerator<EmployeeProductivityDto>, EmployeeProductivityGenerator>();
            servicesCollection.AddTransient<IReportGenerator<PaymentRatioDto>, PaymentMethodRatioGenerator>();

            servicesCollection.AddTransient<IChartGenerator<ProductSalesDto>, SalesChartGenerator>();
            servicesCollection.AddTransient<IChartGenerator<RevenueReportDto>, RevenueChartGenerator>();
            servicesCollection.AddTransient<IChartGenerator<OrderReportDto>, NumberOfOrdersChartGenerator>();
            servicesCollection.AddTransient<IChartGenerator<EmployeeProductivityDto>, EmployeeProductivityChartGenerator>();
            servicesCollection.AddTransient<IChartGenerator<PaymentRatioDto>, PaymentMethodRatioChartGenerator>();

            // Predictions
            servicesCollection.AddTransient<IPredictionGenerator<RevenueReportDto, RevenuePredictionDto>, RevenuePredictionGenerator>();
            servicesCollection.AddTransient<IPredictionGenerator<ProductSalesDto, ProductSalesPredictionDto>, ProductSalesPredictionGenerator>();
            servicesCollection.AddTransient<IPredictionGenerator<OrderReportDto, NumberOfOrdersPredictionDto>, NumberOfOrdersPredictionGenerator>();

            servicesCollection.AddTransient<IChartGenerator<RevenuePredictionDto>, RevenuePredictionChartGenerator>();
            servicesCollection.AddTransient<IChartGenerator<ProductSalesPredictionDto>, SalesPredictionChartGenerator>();
            servicesCollection.AddTransient<IChartGenerator<NumberOfOrdersPredictionDto>, NumberOfOrdersPredictionChartGenerator>();

            // Factories
            servicesCollection.AddSingleton<IReportsFactory, ReportsFactory>();
            servicesCollection.AddTransient<IChartsFactory, ChartsFactory>();
            servicesCollection.AddSingleton<IPredictionsFactory, PredictionsFactory>();

            #endregion

            #region ToDoList

            servicesCollection.AddTransient<ToDoListViewModel>();
            servicesCollection.AddTransient<TaskManagerService>();

            #endregion

            #region LoginPanel

            servicesCollection.AddTransient<SessionService>();
            servicesCollection.AddTransient<LoginManager>();
            servicesCollection.AddTransient<LoginService>();
            servicesCollection.AddTransient<LoginPanelViewModel>();
            servicesCollection.AddTransient<StartFinishWorkViewModel>();
            servicesCollection.AddTransient<WorkTimeSummaryControlViewModel>();

            #endregion

            #region AdministratorFunctions

            servicesCollection.AddTransient<AdminFunctionsService>();
            servicesCollection.AddTransient<AdministratorFunctionsViewModel>();
            servicesCollection.AddTransient<AddEmployeeViewModel>();
            servicesCollection.AddTransient<EditEmployeeViewModel>();

            #endregion

        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                row.IsSelected = !row.IsSelected;
                e.Handled = true;
            }
        }
    }
}