using DataAccess.Models;
using System.Threading.Tasks;
using System.Windows.Input;
using POS.Services.Login;
using System.Collections.ObjectModel;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.WorkTimeSummaryControl
{
    public class WorkTimeSummaryControlViewModel : ViewModelBase
    {
        private readonly SessionService _sessionService;

        private ObservableCollection<EmployeeWorkSession> sessionList = new();

        public ObservableCollection<EmployeeWorkSession> SessionList
        {
            get => sessionList;
            set => SetField(ref sessionList, value);
        }

        public ICommand RefreshCommand { get; }

        public WorkTimeSummaryControlViewModel(SessionService sessionService)
        {
            _sessionService = sessionService;

            RefreshCommand = new RelayCommandAsync(LoadSessionsAsync);

            _ = LoadSessionsAsync();
            _ = CheckForActiveSessionAsync();
        }

        private async Task LoadSessionsAsync()
        {
            SessionList.Clear();
            var sessions = await _sessionService.LoadSessionsAsync();

            foreach (var session in sessions)
            {
                SessionList.Add(session);
            }
        }

        private async Task CheckForActiveSessionAsync()
        {
            await _sessionService.CheckForActiveSessionsAsync();
        }
    }
}
