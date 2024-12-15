using DataAccess.Models;
using System.Threading.Tasks;
using System.Windows.Input;
using POS.Services.Login;
using System.Collections.ObjectModel;
using POS.Utilities;
using POS.Utilities.RelayCommands;

namespace POS.ViewModels.WorkTimeSummaryControl
{
    public class WorkTimeSummaryControlViewModel : ViewModelBase
    {
        private readonly SessionService _sessionService;
        public ObservableCollection<EmployeeWorkSession> SessionList { get; set; }

        public ICommand RefreshCommand { get; }

        public WorkTimeSummaryControlViewModel(SessionService sessionService)
        {
            _sessionService = sessionService;

            SessionList = [];
            RefreshCommand = new RelayCommandAsync(LoadSessionsAsync);

            _ = LoadSessionsAsync();
            _ = CheckForActiveSession();
        }

        private async Task LoadSessionsAsync()
        {
            SessionList.Clear();
            var sessions = await _sessionService.LoadSessions();

            foreach (var session in sessions)
            {
                SessionList.Add(session);
            }
        }

        private async Task CheckForActiveSession()
        {
            await _sessionService.CheckForActiveSessions();
        }
    }
}
