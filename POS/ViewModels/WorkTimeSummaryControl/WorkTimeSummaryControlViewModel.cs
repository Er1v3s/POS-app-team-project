using DataAccess.Models;
using System.Threading.Tasks;
using System.Windows.Input;
using POS.Services.Login;
using System.Collections.ObjectModel;
using POS.Utilities;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.WorkTimeSummaryControl
{
    public class WorkTimeSummaryControlViewModel : ViewModelBase
    {
        private readonly SessionService _sessionService;

        public MyObservableCollection<EmployeeWorkSession> SessionObservableCollection
        {
            get => _sessionService.SessionCollection;
        }

        public ICommand RefreshCommand { get; }

        public WorkTimeSummaryControlViewModel(SessionService sessionService)
        {
            _sessionService = sessionService;

            RefreshCommand = new RelayCommandAsync(LoadSessionsAsync);

            _ = CheckForActiveSessionAsync();
        }

        private async Task LoadSessionsAsync()
        {
            await _sessionService.GetSessionsAsync();
        }

        private async Task CheckForActiveSessionAsync()
        {
            await _sessionService.CheckForActiveSessionsAsync();
        }
    }
}
