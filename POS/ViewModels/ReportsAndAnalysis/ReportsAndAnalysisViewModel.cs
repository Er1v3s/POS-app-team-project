using System;
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

        private SeriesCollection seriesCollection;
        private List<string> labels = [];

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
            set => SetField(ref selectedReportIndex, value);
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

        public SeriesCollection SeriesCollection
        {
            get => seriesCollection;
            set => SetField(ref seriesCollection, value);
        }

        public ICommand GenerateReportCommand { get; }
        public ICommand GeneratePredictionCommand { get; }

        private readonly IReportFactory reportFactory;

        public ReportsAndAnalysisViewModel(IReportFactory reportFactory)
        {
            GenerateReportCommand = new RelayCommand(async _ => await GenerateReport());
            GeneratePredictionCommand = new RelayCommand(async _ => await GeneratePrediction());

            this.reportFactory = reportFactory;
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

            reportFactory.SetParameters(seriesCollection, startDate, endDate);
            await reportFactory.GenerateReport(selectedReportIndex);

            labels = reportFactory.GetUpdatedLabelsValues();
            OnPropertyChanged(nameof(labels));
        }

        private async Task GeneratePrediction()
        {
            seriesCollection.Clear();

            //
            reportFactory.SetParameters(seriesCollection, DateTime.Now.AddDays(-56), DateTime.Now.AddDays(-28)); // TO CHANGE
            //

            await reportFactory.GeneratePrediction(selectedReportIndex);

            labels = reportFactory.GetUpdatedLabelsValues();
            OnPropertyChanged(nameof(labels));
        }
    }
}