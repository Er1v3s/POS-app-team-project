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
using POS.Services.ReportsAndAnalysis.ChartGenerators.PredictionChartGenerators;
using POS.Services.ReportsAndAnalysis.ChartGenerators.ReportChartGenerators;
using POS.Services.ReportsAndAnalysis.Factories;
using POS.Services.ReportsAndAnalysis.Interfaces;
using POS.Services.ReportsAndAnalysis.PredictionGenerators;
using POS.Services.ReportsAndAnalysis.ReportGenerators;
using POS.Services.SalesPanel;
using POS.Services.ToDoList;
using POS.Services.WarehouseFunctions;
using POS.ViewModels.AdminFunctionsPanel;
using POS.ViewModels.MainWindow;
using POS.ViewModels.ReportsAndAnalysis;
using POS.ViewModels.SalesPanel;
using POS.ViewModels.StartFinishWork;
using POS.ViewModels.ToDoList;
using POS.ViewModels.WarehouseFunctions;
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
            servicesCollection.AddSingleton<IReportsFactory, ReportFactory>();
            servicesCollection.AddTransient<IChartsFactory, ChartFactory>();
            servicesCollection.AddSingleton<IPredictionsFactory, PredictionFactory>();

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
            servicesCollection.AddTransient<AdminFunctionsViewModel>();
            servicesCollection.AddTransient<AddEmployeeViewModel>();
            servicesCollection.AddTransient<EditEmployeeViewModel>();

            #endregion

            #region SalesPanel

            servicesCollection.AddScoped<ProductService>();
            servicesCollection.AddTransient<OrderService>();
            servicesCollection.AddTransient<RecipeService>();
            servicesCollection.AddTransient<OrderSummaryService>();
            servicesCollection.AddTransient<FinishedOrderService>();
            servicesCollection.AddScoped<IngredientService>();
            servicesCollection.AddScoped<InvoiceService>();
            servicesCollection.AddScoped<DiscountService>();

            servicesCollection.AddTransient<DiscountWindowViewModel>();
            servicesCollection.AddTransient<OrderSummaryViewModel>();
            servicesCollection.AddTransient<FinishedOrderViewModel>();
            servicesCollection.AddTransient<SalesPanelViewModel>();
            servicesCollection.AddTransient<InvoiceViewModel>();


            #endregion

            #region WarehouseFunctions

            servicesCollection.AddTransient<DeliveryService>();

            servicesCollection.AddTransient<WarehouseFunctionsViewModel>();

            servicesCollection.AddTransient<EditProductRecipeViewModel>();
            servicesCollection.AddTransient<AddEditDeleteProductViewModel>();
            servicesCollection.AddTransient<AddEditDeleteIngredientViewModel>();
            servicesCollection.AddTransient<StockManagementViewModel>();

            servicesCollection.AddTransient<CreateDeliveryViewModel>();
            servicesCollection.AddTransient<StockCorrectionViewModel>();

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