using System.Linq;
using System;
using System.Windows;
using POS.Models.Reports.ReportsPredictions;
using POS.Models.Reports;

namespace POS.ViewModels.ReportsAndAnalysis.Metrics
{
    public class MetricsCalculator
    {
        public static void CalculateMetrics(IQueryable<RevenueReportDto> actualData, IQueryable<RevenuePredictionDto> predictedData)
        {
            var actualValues = actualData.Select(d => d.TotalRevenue).ToArray();
            var predictedValues = predictedData.Select(p => p.TotalRevenue).ToArray();

            MessageBox.Show($"Rzeczywiste dane: {actualValues.Length}, Prognozowane dane: {predictedValues.Length}");

            if (actualValues.Length != predictedValues.Length)
            {
                Console.WriteLine("Nie można obliczyć metryk: różne długości rzeczywistych i prognozowanych danych.");
                return;
            }

            var mae = actualValues.Zip(predictedValues, (actual, predicted) => Math.Abs(actual - predicted)).Average();
            var mse = actualValues.Zip(predictedValues, (actual, predicted) => Math.Pow(actual - predicted, 2)).Average();
            var rmse = Math.Sqrt(mse);
            var meanActual = actualValues.Average();
            var ssTotal = actualValues.Sum(actual => Math.Pow(actual - meanActual, 2));
            var ssResidual = actualValues.Zip(predictedValues, (actual, predicted) => Math.Pow(actual - predicted, 2)).Sum();
            var rSquared = 1 - (ssResidual / ssTotal);

            MessageBox.Show($"AVG: {meanActual}\n\nMAE: {mae},\n\nMSE: {mse},\n\nRMSE: {rmse},\n\nR²: {rSquared}");
        }
    }
}
