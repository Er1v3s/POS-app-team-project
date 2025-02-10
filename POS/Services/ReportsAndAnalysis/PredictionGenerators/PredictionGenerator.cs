using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using POS.Models.Reports;
using POS.Models.Reports.ReportsPredictions;

namespace POS.Services.ReportsAndAnalysis.PredictionGenerators
{
    public abstract class PredictionGenerator<TDto> where TDto : IReportDto
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        protected PredictionGenerator()
        {
            _mlContext = new MLContext();
        }

        protected void TrainModel(IEnumerable<PredictionInput> data, int windowSize, int horizon)
        {
            var trainData = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(PredictionDataModel.Total),
                inputColumnName: nameof(PredictionInput.Value),
                windowSize: windowSize,
                seriesLength: data.Count(),
                trainSize: (int)Math.Round(data.Count() * 0.8),
                horizon: horizon);

            _model = pipeline.Fit(trainData);
        }

        protected IEnumerable<PredictionInput> PrepareTimeSeriesData(IEnumerable<TDto> data)
        {
            return data
                .Select(g => new PredictionInput
                {
                    Date = g.Date,
                    Value = g.Value
                });
        }

        protected async Task<PredictionDataModel> GenerateForecast()
        {
            var forecastEngine = _model.CreateTimeSeriesEngine<PredictionInput, PredictionDataModel>(_mlContext);
            var forecast = await Task.Run(() => forecastEngine.Predict());

            return forecast;
        }
    }
}
