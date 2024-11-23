﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;
using POS.ViewModels;
using POS.ViewModels.ReportsAndAnalysis;
using POS.ViewModels.ReportsAndAnalysis.ChartGenerators.PredictionChartGenerators;
using POS.ViewModels.ReportsAndAnalysis.ChartGenerators.ReportChartGenerators;
using POS.ViewModels.ReportsAndAnalysis.Factories;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;
using POS.ViewModels.ReportsAndAnalysis.PredictionGenerators;
using POS.ViewModels.ReportsAndAnalysis.PredictionGenerators.POS.ViewModels.ReportsAndAnalysis.PredictionGenerators;
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

            // Reports
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

            // Predictions
            servicesCollection.AddSingleton<IPredictionGenerator<RevenueReportDto, RevenuePredictionDto>, RevenuePredictionGenerator>();
            servicesCollection.AddSingleton<IPredictionGenerator<ProductSalesDto, ProductSalesPredictionDto>, ProductSalesPredictionGenerator>();

            servicesCollection.AddSingleton<IChartGenerator<RevenuePredictionDto>, RevenuePredictionChartGenerator>();
            servicesCollection.AddSingleton<IChartGenerator<ProductSalesPredictionDto>, SalesPredictionChartGenerator>();

            // Factories
            servicesCollection.AddSingleton<IReportsFactory, ReportsFactory>();
            servicesCollection.AddSingleton<IChartsFactory, ChartsFactory>();
            servicesCollection.AddSingleton<IPredictionsFactory, PredictionsFactory>();
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