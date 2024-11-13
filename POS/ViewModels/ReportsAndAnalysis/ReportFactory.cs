using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;
using SeriesCollection = LiveCharts.SeriesCollection;

namespace POS.ViewModels.ReportsAndAnalysis
{
    public class ReportFactory : IReportFactory
    {
        private readonly Dictionary<int, Func<Task>> reportGenerators;

        private SeriesCollection seriesCollection;
        private DateTime startDate;
        private DateTime endDate;
        private List<string> labels;

        public ReportFactory(
            IReportGenerator<ProductSalesDto> saleReportGenerator,
            IReportGenerator<RevenueReportDto> revenueReportGenerator,
            IReportGenerator<OrderReportDto> numberOfOrdersReportGenerator,
            IReportGenerator<EmployeeProductivityDto> employeeProductivityReportGenerator,
            IReportGenerator<PaymentRatioDto> paymentRatioReportGenerator,

            IChartGenerator<ProductSalesDto> salesReportChartGenerator,
            IChartGenerator<RevenueReportDto> revenueReportChartGenerator,
            IChartGenerator<OrderReportDto> numberOfOrdersReportChartGenerator,
            IChartGenerator<EmployeeProductivityDto> employeeProductivityReportChartGenerator,
            IChartGenerator<PaymentRatioDto> paymentRatioReportChartGenerator)
        {
            reportGenerators = new Dictionary<int, Func<Task>>
            {
                { 0, async () => await GenerateReport(saleReportGenerator, salesReportChartGenerator) },
                { 1, async () => await GenerateReport(revenueReportGenerator, revenueReportChartGenerator, "day", r => r.Date.ToString("yyyy-MM-dd")) },
                { 2, async () => await GenerateReport(revenueReportGenerator, revenueReportChartGenerator, "week", r => r.DayOfWeek.ToString()) },
                { 3, async () => await GenerateReport(revenueReportGenerator, revenueReportChartGenerator, "month", r => r.Date.ToString("yyyy-MM")) },
                { 4, async () => await GenerateReport(revenueReportGenerator, revenueReportChartGenerator, "year", r => r.Date.ToString("yyyy")) },
                { 5, async () => await GenerateReport(numberOfOrdersReportGenerator, numberOfOrdersReportChartGenerator, "day", o => o.Date.ToString("yyyy-MM-dd")) },
                { 6, async () => await GenerateReport(numberOfOrdersReportGenerator, numberOfOrdersReportChartGenerator, "week", o => o.DayOfWeek.ToString()) },
                { 7, async () => await GenerateReport(numberOfOrdersReportGenerator, numberOfOrdersReportChartGenerator, "month", o => o.Date.ToString("yyyy-MM")) },
                { 8, async () => await GenerateReport(numberOfOrdersReportGenerator, numberOfOrdersReportChartGenerator, "year", o => o.Date.ToString("yyyy")) },
                { 9, async () => await GenerateReport(employeeProductivityReportGenerator, employeeProductivityReportChartGenerator) },
                { 10, async () => await GenerateReport(paymentRatioReportGenerator, paymentRatioReportChartGenerator) }
            };
        }

        private async Task GenerateReport<T>(IReportGenerator<T> reportGenerator, IChartGenerator<T> chartGenerator, string? groupBy = null, Func<dynamic, string>? labelSelector = null)
        {
            var data = await reportGenerator.GenerateData(startDate, endDate, groupBy);
            chartGenerator.GenerateChart(data, seriesCollection, out labels, labelSelector);
        }

        public void SetParameters(SeriesCollection seriesCollection, DateTime startDate, DateTime endDate)
        {
            this.seriesCollection = seriesCollection;
            this.startDate = startDate;
            this.endDate = endDate;
        }

        public async Task GenerateReport(int selectedReportIndex)
        {
            await reportGenerators[selectedReportIndex]();
        }

        public List<string> GetUpdatedLabelsValues()
        {
            return labels;
        }
    }
}
