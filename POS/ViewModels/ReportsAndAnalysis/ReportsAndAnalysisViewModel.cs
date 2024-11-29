﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LiveCharts;
using POS.Models.Reports;
using POS.ViewModels.ReportsAndAnalysis.Interfaces;
using POS.ViewModels.ReportsAndAnalysis.Validators;

namespace POS.ViewModels.ReportsAndAnalysis
{
    public class ReportsAndAnalysisViewModel : ViewModelBase
    {
        private int selectedReportIndex;
        private DateTime startDate = DateTime.Now.AddMonths(-1);
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
                UpdateControlsStatus();
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
            GenerateReportCommand = new RelayCommand(async _ => await GenerateReport());
            GeneratePredictionCommand = new RelayCommand(async _ => await GeneratePrediction());

            _reportFactory = reportFactory;
            _chartFactory = chartFactory;
            _predictionsFactory = predictionsFactory;

            seriesCollection = new SeriesCollection();

            _ = GenerateReport(); // default report
        }

        private async Task GenerateReport()
        {
            var inputValidator = new InputValidator();
            var validationResult = inputValidator.ValidateInputs(selectedReportIndex, startDate, endDate);

            if (!validationResult.IsValid)
            {
                MessageBox.Show(validationResult.ErrorMessage);
                return;
            }

            seriesCollection.Clear();

            SetStartAndEndDateInReportsFactory(selectedReportIndex);
            await _reportFactory.GenerateReport(selectedReportIndex);
            await _chartFactory.GenerateChart(selectedReportIndex, seriesCollection, ChartType.Report);

            labels = _chartFactory.GetUpdatedLabelsValues();
            OnPropertyChanged(nameof(labels));
        }

        private async Task GeneratePrediction()
        {
            seriesCollection.Clear();

            await _predictionsFactory.GeneratePrediction(selectedReportIndex, seriesCollection);
            await _chartFactory.GenerateChart(selectedReportIndex, seriesCollection, ChartType.Prediction);

            labels = _chartFactory.GetUpdatedLabelsValues();
            OnPropertyChanged(nameof(labels));
        }

        private void UpdateControlsStatus()
        {
            isDatePickerControlsEnabled = selectedReportIndex is not (11 or 12 or 13);
            OnPropertyChanged(nameof(isDatePickerControlsEnabled));

            isAiPredictionControlsEnabled = selectedReportIndex is not (0 or 9 or 10);
            OnPropertyChanged(nameof(isAiPredictionControlsEnabled));
        }

        private void SetStartAndEndDateInReportsFactory(int index)
        {
            // AddMonths(-2) is temporary because there is no data in database
            startDate = index switch
            {
                11 => DateTime.Now.AddDays(-7).AddMonths(-2),
                12 => DateTime.Now.AddMonths(-2),
                13 => DateTime.Now.AddYears(-1).AddMonths(-2),
                _ => startDate
            };

            if (index is 11 or 12 or 13)
                endDate = DateTime.Now;

            _reportFactory.SetParameters(startDate, endDate);
        }
    }
}