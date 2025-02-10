using DataAccess.Models;
using System.Threading.Tasks;
using System.Windows.Input;
using POS.Services.Login;
using POS.Exceptions;
using POS.Utilities;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.WorkTimeSummaryControl
{
    public class WorkTimeSummaryControlViewModel : ViewModelBase
    {
        private readonly SessionService _sessionService;

        public MyObservableCollection<EmployeeWorkSession> SessionObservableCollection => _sessionService.SessionCollection;

        public ICommand RefreshCommand { get; }

        public WorkTimeSummaryControlViewModel(SessionService sessionService)
        {
            _sessionService = sessionService;

            RefreshCommand = new RelayCommandAsync(LoadSessionsAsync);
        }

        private async Task LoadSessionsAsync()
        {
            try
            {
                IsButtonEnable = !IsButtonEnable;
                await _sessionService.GetSessionsAsync();
            }
            catch (DatabaseException ex)
            {
                ExceptionsHandler.ShowErrorMessage(ex.Message);
            }
            finally
            {
                IsButtonEnable = !IsButtonEnable;
            }
        }
    }
}
