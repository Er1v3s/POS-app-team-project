using System;
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
                { 1, async () => await GenerateReportData(revenueReportGenerator, "day")},
                { 2, async () => await GenerateReportData(revenueReportGenerator, "week") },
                { 3, async () => await GenerateReportData(revenueReportGenerator, "month") },
                { 4, async () => await GenerateReportData(revenueReportGenerator, "year") },
                { 5, async () => await GenerateReportData(numberOfOrdersReportGenerator, "day") },
                { 6, async () => await GenerateReportData(numberOfOrdersReportGenerator, "week") },
                { 7, async () => await GenerateReportData(numberOfOrdersReportGenerator, "month") },
                { 8, async () => await GenerateReportData(numberOfOrdersReportGenerator, "year") },
                { 9, async () => await GenerateReportData(employeeProductivityReportGenerator) },
                { 10, async () => await GenerateReportData(paymentRatioReportGenerator) }
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

        private async Task GenerateReportData<T>(IReportGenerator<T> reportGenerator, string? groupBy = null)
        {
            reportData = await reportGenerator.GenerateData(startDate, endDate, groupBy);
        }
    }
}
