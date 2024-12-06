using System;
using System.Collections.Generic;
using System.Linq;
using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.PredictionGenerators
{
    public class RevenuePredictionGenerator : PredictionGenerator<RevenueReportDto>, IPredictionGenerator<RevenueReportDto, RevenuePredictionDto>
    {
        public IQueryable<RevenuePredictionDto> GeneratePrediction(IQueryable<RevenueReportDto> data, int windowSize, int horizon, GroupBy groupBy)
        {
            var timeSeriesData = PrepareTimeSeriesData(data);

            TrainModel(timeSeriesData, windowSize, horizon);

            var prediction = Predict(groupBy);

            return prediction;
        }

        private IQueryable<RevenuePredictionDto> Predict(GroupBy groupBy)
        {
            var forecast = GenerateForecast();

            var formattedPrediction = SetDataFormat(forecast, groupBy);

            return formattedPrediction;
        }

        private IQueryable<RevenuePredictionDto> SetDataFormat(PredictionDataModel forecast, GroupBy groupBy)
        {
            var predictions = new List<RevenuePredictionDto>();

            for (int i = 0; i < forecast.Total.Length; i++)
            {
                switch (groupBy)
                {
                    case GroupBy.Day:
                        predictions.Add(new RevenuePredictionDto
                        {
                            Date = DateTime.Now.AddDays(i + 1),
                            TotalRevenue = forecast.Total[i]
                        });
                        break;
                    case GroupBy.Month:
                        predictions.Add(new RevenuePredictionDto
                        {
                            Date = DateTime.Now.AddMonths(i + 1),
                            TotalRevenue = forecast.Total[i]
                        });
                        break;
                    case GroupBy.Year:
                        predictions.Add(new RevenuePredictionDto
                        {
                            Date = DateTime.Now.AddYears(i + 1),
                            TotalRevenue = forecast.Total[i]
                        });
                        break;
                }
            }

            return predictions.AsQueryable();
        }
    }
}
