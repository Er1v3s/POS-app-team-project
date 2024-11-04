using POS.Models;
using POS.Services;
using System.Collections.Generic;
using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts;

namespace POS.ViewModels
{
    public class SalesPredictionViewModel
    {
        private readonly SalesPredictionService _predictionService;

        public RevenuePrediction Forecast { get; private set; }

        public SalesPredictionViewModel()
        {
            _predictionService = new SalesPredictionService();
        }

        public void TrainAndPredict(List<RevenueReportDto> revenueData)
        {
            // Trenowanie modelu
            var model = _predictionService.TrainModel(revenueData);

            // Generowanie prognozy
            Forecast = _predictionService.Predict(model, revenueData);
        }
    }
}