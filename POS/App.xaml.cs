using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;
using POS.ViewModels;
using POS.ViewModels.ReportsAndAnalysis;
using POS.ViewModels.ReportsAndAnalysis.ChartGenerators;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;
using POS.ViewModels.ReportsAndAnalysis.Predictions;
using POS.ViewModels.ReportsAndAnalysis.ReportGenerators;

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
            servicesCollection.AddSingleton<ReportsAndAnalysisViewModel>();

            servicesCollection.AddSingleton<IReportGenerator<ProductSalesDto>, SalesReportGenerator>();
            servicesCollection.AddSingleton<IReportGenerator<RevenueReportDto>, RevenueReportGenerator>();
            servicesCollection.AddSingleton<IReportGenerator<OrderReportDto>, NumberOfOrdersGenerator>();
            servicesCollection.AddSingleton<IReportGenerator<EmployeeProductivityDto>, EmployeeProductivityGenerator>();
            servicesCollection.AddSingleton<IReportGenerator<PaymentRatioDto>, CardToCashPaymentRatioGenerator>();

            servicesCollection.AddSingleton<IChartGenerator<ProductSalesDto>, SalesChartGenerator>();
            servicesCollection.AddSingleton<IChartGenerator<RevenueReportDto>, RevenueChartGenerator>();
            servicesCollection.AddSingleton<IChartGenerator<OrderReportDto>, NumberOfOrdersChartGenerator>();
            servicesCollection.AddSingleton<IChartGenerator<EmployeeProductivityDto>, EmployeeProductivityChartGenerator>();
            servicesCollection.AddSingleton<IChartGenerator<PaymentRatioDto>, PaymentMethodRatioChartGenerator>();

            servicesCollection.AddSingleton<IChartGenerator<RevenuePredictionDto>, PredictionChart>();

            servicesCollection.AddSingleton<IReportFactory, ReportFactory>();
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