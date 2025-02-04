using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.Factories
{
    public class ReportFactory : IReportsFactory
    {
        private readonly Dictionary<int, Func<Task>> _reportDataGenerators;

        private DateTime startDate;
        private DateTime endDate;

        private object reportData;

        public ReportFactory(
            IReportGenerator<ProductSalesDto> saleReportGenerator,
            IReportGenerator<RevenueReportDto> revenueReportGenerator,
            IReportGenerator<OrderReportDto> numberOfOrdersReportGenerator,
            IReportGenerator<EmployeeProductivityDto> employeeProductivityReportGenerator,
            IReportGenerator<PaymentRatioDto> paymentRatioReportGenerator
                )
        {
            _reportDataGenerators = new Dictionary<int, Func<Task>>
            {
                { 0, async () => await GenerateReportData(saleReportGenerator, GroupBy.Day) },
                { 1, async () => await GenerateReportData(saleReportGenerator, GroupBy.Day) },
                { 2, async () => await GenerateReportData(saleReportGenerator, GroupBy.Month) },
                { 3, async () => await GenerateReportData(saleReportGenerator, GroupBy.Year) },
                { 4, async () => await GenerateReportData(revenueReportGenerator, GroupBy.Day)},
                { 5, async () => await GenerateReportData(revenueReportGenerator, GroupBy.Week) },
                { 6, async () => await GenerateReportData(revenueReportGenerator, GroupBy.Month) },
                { 7, async () => await GenerateReportData(revenueReportGenerator, GroupBy.Year) },
                { 8, async () => await GenerateReportData(numberOfOrdersReportGenerator, GroupBy.Day) },
                { 9, async () => await GenerateReportData(numberOfOrdersReportGenerator, GroupBy.Week) },
                { 10, async () => await GenerateReportData(numberOfOrdersReportGenerator, GroupBy.Month) },
                { 11, async () => await GenerateReportData(numberOfOrdersReportGenerator, GroupBy.Year) },
                { 12, async () => await GenerateReportData(employeeProductivityReportGenerator) },
                { 13, async () => await GenerateReportData(paymentRatioReportGenerator) },
            };
        }

        public void SetParameters(DateTime startDate, DateTime endDate)
        {
            this.startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            this.endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day).AddDays(1).AddSeconds(-1);
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
