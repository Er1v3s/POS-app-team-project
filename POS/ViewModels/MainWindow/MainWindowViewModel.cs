using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using POS.Services;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;
using POS.Views.Windows.SalesPanel;

namespace POS.ViewModels.MainWindow
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly TimeService _timeService;
        private readonly NavigationService _navigationService;
        private readonly ApplicationStateService _applicationStateService;

        private object contentSource;

        public Visibility IsInternetAvailable => 
            _applicationStateService.IsInternetAvailable ? Visibility.Collapsed : Visibility.Visible;

        public Visibility IsDatabaseAvailable =>
            _applicationStateService.IsDatabaseAvailable ? Visibility.Collapsed : Visibility.Visible;

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
        public ICommand OpenSalesPanelWindowCommand { get; }
        public ICommand OpenLoginPanelWindowCommand { get; }
        public ICommand TurnOffApplicationCommand { get; }
        public ICommand ChangeContentSourceCommand { get; }

        public MainWindowViewModel(
            TimeService timeService,
            NavigationService navigationService,
            ApplicationStateService applicationStateService)
        {
            _timeService = timeService;
            InitializeTimeService();

            _navigationService = navigationService;
            _applicationStateService = applicationStateService;

            _applicationStateService.PropertyChanged += OnApplicationStateChanged;

            TurnOffApplicationCommand = new RelayCommand(TurnOffApplication);
            OpenSalesPanelWindowCommand = new RelayCommand<SalesPanelWindow>(OpenSalesPanelWindow);
            OpenLoginPanelWindowCommand = new RelayCommand(OpenLoginPanelWindow);
            ChangeContentSourceCommand = new RelayCommand(ChangeContentSource);

            SetDefaultContentSource(0);
        }

        public void OpenSalesPanelWindow<T>(T windowType)
        {
            _navigationService.OpenWindow(windowType);

            if (Application.Current.Windows.OfType<SalesPanelWindow>().Any())
                CloseWindowBaseAction!.Invoke();
        }

        public void OpenLoginPanelWindow()
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

        private void OnApplicationStateChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ApplicationStateService.IsInternetAvailable))
                OnPropertyChanged(nameof(IsInternetAvailable));
            
            if(e.PropertyName == nameof(ApplicationStateService.IsDatabaseAvailable))
                OnPropertyChanged(nameof(IsDatabaseAvailable));
        }

        private void TurnOffApplication(object obj)
        {
            TurnOffApplicationAction.Invoke();
        }
    }
}
