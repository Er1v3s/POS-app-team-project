﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.Factories
{
    public class ReportsFactory : IReportsFactory
    {
        private readonly Dictionary<int, Func<Task>> _reportDataGenerators;

        private DateTime startDate;
        private DateTime endDate;

        private object reportData;

        public ReportsFactory(
            IReportGenerator<ProductSalesDto> saleReportGenerator,
            IReportGenerator<RevenueReportDto> revenueReportGenerator,
            IReportGenerator<OrderReportDto> numberOfOrdersReportGenerator,
            IReportGenerator<EmployeeProductivityDto> employeeProductivityReportGenerator,
            IReportGenerator<PaymentRatioDto> paymentRatioReportGenerator
                )
        {
            _reportDataGenerators = new Dictionary<int, Func<Task>>
            {
                { 0, async () => await GenerateReportData(saleReportGenerator) },
                { 1, async () => await GenerateReportData(revenueReportGenerator, GroupBy.Day)},
                { 2, async () => await GenerateReportData(revenueReportGenerator, GroupBy.Week) },
                { 3, async () => await GenerateReportData(revenueReportGenerator, GroupBy.Month) },
                { 4, async () => await GenerateReportData(revenueReportGenerator, GroupBy.Year) },
                { 5, async () => await GenerateReportData(numberOfOrdersReportGenerator, GroupBy.Day) },
                { 6, async () => await GenerateReportData(numberOfOrdersReportGenerator, GroupBy.Week) },
                { 7, async () => await GenerateReportData(numberOfOrdersReportGenerator, GroupBy.Month) },
                { 8, async () => await GenerateReportData(numberOfOrdersReportGenerator, GroupBy.Year) },
                { 9, async () => await GenerateReportData(employeeProductivityReportGenerator) },
                { 10, async () => await GenerateReportData(paymentRatioReportGenerator) },
                { 11, async () => await GenerateReportData(saleReportGenerator) },
                { 12, async () => await GenerateReportData(saleReportGenerator) },
                { 13, async () => await GenerateReportData(saleReportGenerator) },
            };
        }

        public void SetParameters(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }

        public async Task GenerateReport(int selectedReportIndex)
        {
            await _reportDataGenerators[selectedReportIndex]();
        }

        public object GetReportData()
        {
            return reportData;
        }

        private async Task GenerateReportData<T>(IReportGenerator<T> reportGenerator, GroupBy? groupBy = null)
        {
            reportData = await reportGenerator.GenerateData(startDate, endDate, groupBy);
        }
    }
}
