using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LiveCharts;
using POS.Models.Reports;
using POS.Services.ReportsAndAnalysis.Interfaces;
using POS.Utilities.RelayCommands;
using POS.Validators;
using POS.ViewModels.Base;

namespace POS.ViewModels.ReportsAndAnalysis
{
    public class ReportsAndAnalysisViewModel : ViewModelBase
    {
        private int selectedReportIndex;
        private DateTime startDate = DateTime.Now.AddMonths(-2);
        //private DateTime startDate = DateTime.Now.AddDays(-1);
        private DateTime endDate = DateTime.Now;
        private bool isDatePickerControlsEnabled = true;
        private bool isAiPredictionControlsEnabled;

        private SeriesCollection seriesCollection;
        private List<string> labels = [];

        private readonly IReportsFactory _reportFactory;
        private readonly IChartsFactory _chartFactory;
        private readonly IPredictionsFactory _predictionsFactory;

        public List<string> Labels
        {
            get => labels;
            set
            {
                SetField(ref labels, value);
                OnPropertyChanged(nameof(labels));
            }
        }

        public int SelectedReportIndex
        {
            get => selectedReportIndex;
            set
            {
                SetField(ref selectedReportIndex, value);
                UpdateControlsStatus(selectedReportIndex);
                SetStartAndEndDate(selectedReportIndex);
            }
        }

        public DateTime StartDate
        {
            get => startDate;
            set => SetField(ref startDate, value);
        }

        public DateTime EndDate
        {
            get => endDate;
            set => SetField(ref endDate, value);
        }

        public bool IsDatePickerControlsEnabled
        {
            get => isDatePickerControlsEnabled;
            set => SetField(ref isDatePickerControlsEnabled, value);
        }

        public bool IsAiPredictionControlsEnabled
        {
            get => isAiPredictionControlsEnabled;
            set => SetField(ref isAiPredictionControlsEnabled, value);
        }

        public SeriesCollection SeriesCollection
        {
            get => seriesCollection;
            set => SetField(ref seriesCollection, value);
        }

        public ICommand GenerateReportCommand { get; }
        public ICommand GeneratePredictionCommand { get; }

        public ReportsAndAnalysisViewModel(IReportsFactory reportFactory, IChartsFactory chartFactory, IPredictionsFactory predictionsFactory)
        {
            GenerateReportCommand = new RelayCommandAsync(GenerateReport);
            GeneratePredictionCommand = new RelayCommandAsync(GeneratePrediction);

            _reportFactory = reportFactory;
            _chartFactory = chartFactory;
            _predictionsFactory = predictionsFactory;

            seriesCollection = new SeriesCollection();

            _ = GenerateReport(); // default report
        }

        private async Task GenerateReport()
        {
            var validationResult = DateIntervalValidator.ValidateDateInterval(startDate, endDate);

            if (validationResult.Result == false)
            {
                MessageBox.Show(validationResult.ErrorMessage);
                return;
            }

            seriesCollection.Clear();

            SetStartAndEndDate(selectedReportIndex);
            _reportFactory.SetParameters(startDate, endDate);
            await _reportFactory.GenerateReport(selectedReportIndex);
            _chartFactory.GenerateChart(selectedReportIndex, seriesCollection, ChartType.Report);

            labels = _chartFactory.GetUpdatedLabelsValues();
            OnPropertyChanged(nameof(labels));
        }

        private async Task GeneratePrediction()
        {
            seriesCollection.Clear();

            await _predictionsFactory.GeneratePrediction(selectedReportIndex, seriesCollection);
            _chartFactory.GenerateChart(selectedReportIndex, seriesCollection, ChartType.Prediction);

            labels = _chartFactory.GetUpdatedLabelsValues();
            OnPropertyChanged(nameof(labels));
        }

        private void UpdateControlsStatus(int index)
        {
            isDatePickerControlsEnabled = index is not (1 or 2 or 3);
            OnPropertyChanged(nameof(isDatePickerControlsEnabled));

            isAiPredictionControlsEnabled = selectedReportIndex is not (0 or 12 or 13);
            OnPropertyChanged(nameof(isAiPredictionControlsEnabled));
        }

        private void SetStartAndEndDate(int index)
        {
            // AddMonths(-2) is temporary because there is no data in database

            startDate = index switch
            {
                1 => DateTime.Now.AddDays(-7).AddMonths(-2),
                2 => DateTime.Now.AddMonths(-1).AddMonths(-2),
                3 => DateTime.Now.AddYears(-1).AddMonths(-2),
                _ => StartDate
            };

            endDate = index switch
            {
                1 => DateTime.Now.AddMonths(-2),
                2 => DateTime.Now.AddMonths(-2),
                3 => DateTime.Now.AddMonths(-2),
                _ => EndDate
            };

            //startDate = index switch
            //{
            //    1 => DateTime.Now.AddDays(-7),
            //    2 => DateTime.Now.AddMonths(-1),
            //    3 => DateTime.Now.AddYears(-1),
            //    _ => StartDate
            //};

            //endDate = index switch
            //{
            //    1 => DateTime.Now,
            //    2 => DateTime.Now,
            //    3 => DateTime.Now,
            //    _ => EndDate
            //};

            OnPropertyChanged(nameof(startDate));
            OnPropertyChanged(nameof(endDate));
        }
    }
}