﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;
using POS.ViewModels.ReportsAndAnalysis;
using POS.ViewModels.ReportsAndAnalysis.ChartGenerators.PredictionChartGenerators;
using POS.ViewModels.ReportsAndAnalysis.ChartGenerators.ReportChartGenerators;
using POS.ViewModels.ReportsAndAnalysis.Factories;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;
using POS.ViewModels.ReportsAndAnalysis.PredictionGenerators;
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