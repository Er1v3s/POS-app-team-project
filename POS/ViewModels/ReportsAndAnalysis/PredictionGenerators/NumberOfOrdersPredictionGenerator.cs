using System;
using System.Collections.Generic;
using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;

namespace POS.ViewModels.ReportsAndAnalysis.PredictionGenerators
{
    public class NumberOfOrdersPredictionGenerator : PredictionGenerator<OrderReportDto>, IPredictionGenerator<OrderReportDto, NumberOfOrdersPredictionDto>
    {
        public List<NumberOfOrdersPredictionDto> GeneratePrediction(List<OrderReportDto> data, int windowSize, int horizon, GroupBy groupBy)
        {
            var timeSeriesData = PrepareTimeSeriesData(data);

            TrainModel(timeSeriesData, windowSize, horizon);

            var prediction = Predict(groupBy);

            return prediction;
        }

        private List<NumberOfOrdersPredictionDto> Predict(GroupBy groupBy)
        {
            var forecast = GenerateForecast();

            var formattedPrediction = SetDataFormat(forecast, groupBy);

            return formattedPrediction;
        }

        private List<NumberOfOrdersPredictionDto> SetDataFormat(PredictionDataModel forecast, GroupBy groupBy)
        {
            var predictions = new List<NumberOfOrdersPredictionDto>();

            for (int i = 0; i < forecast.Total.Length; i++)
            {
                switch (groupBy)
                {
                    case GroupBy.Day:
                        predictions.Add(new NumberOfOrdersPredictionDto
                        {
                            Date = DateTime.Now.AddDays(i + 1),
                            NumberOfOrders = (int)forecast.Total[i]
                        });
                        break;
                    case GroupBy.Month:
                        predictions.Add(new NumberOfOrdersPredictionDto
                        {
                            Date = DateTime.Now.AddMonths(i + 1),
                            NumberOfOrders = (int)forecast.Total[i]
                        });
                        break;
                    case GroupBy.Year:
                        predictions.Add(new NumberOfOrdersPredictionDto
                        {
                            Date = DateTime.Now.AddYears(i + 1),
                            NumberOfOrders = (int)forecast.Total[i]
                        });
                        break;
                }
            }

            return predictions;
        }
    }
}
