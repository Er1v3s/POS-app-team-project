using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using POS.Services;
using POS.Utilities;
using POS.Utilities.RelayCommands;
using POS.Views.RegisterSale;

namespace POS.ViewModels.MainWindow
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly TimeService _timeService;
        private readonly NavigationService _navigationService;

        private object contentSource;

        public DateTime Date { get; private set; }
        public DateTime Time { get; private set; }

        public object ContentSource
        {
            get => contentSource;
            set
            {
                SetField(ref contentSource, value);
                OnPropertyChanged();
            }
        }

        public Action TurnOffApplicationAction;
        public Action CloseWindowsAction;
        public ICommand OpenSalesPanelWindowCommand { get; }
        public ICommand OpenLoginPanelWindowCommand { get; }
        public ICommand TurnOffApplicationCommand { get; }
        public ICommand ChangeContentSourceCommand { get; }

        public MainWindowViewModel(TimeService timeService, NavigationService navigationService)
        {
            _timeService = timeService;
            InitializeTimeService();

            _navigationService = navigationService;

            TurnOffApplicationCommand = new RelayCommand(TurnOffApplication);
            OpenSalesPanelWindowCommand = new RelayCommand(OpenSalesPanelWindow);
            OpenLoginPanelWindowCommand = new RelayCommand(OpenLoginPanelWindow);
            ChangeContentSourceCommand = new RelayCommand(ChangeContentSource);

            SetDefaultContentSource(0);
        }

        public void OpenSalesPanelWindow(object obj)
        {
            _navigationService.OpenSalesPanelWindow();

            if (Application.Current.Windows.OfType<SalesPanel>().Any())
                CloseWindowsAction.Invoke();
        }

        public void OpenLoginPanelWindow(object obj)
        {
            _navigationService.OpenLoginPanelWindow();
        }

        private void ChangeContentSource(object commandParameter)
        {
            try
            {
                ContentSource = _navigationService.GetViewSource(commandParameter);
            } catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void SetDefaultContentSource(object sourceNumber)
        {
            ChangeContentSource(sourceNumber);
        }

        private void InitializeTimeService()
        {
            _timeService.TimeUpdated += UpdateDateTime;
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            Date = _timeService.CurrentTime.Date;
            Time = _timeService.CurrentTime.ToLocalTime();
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(Time));
        }

        private void TurnOffApplication(object obj)
        {
            TurnOffApplicationAction.Invoke();
        }
    }
}
