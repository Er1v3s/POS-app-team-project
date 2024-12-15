using System.Threading.Tasks;
using System.Windows.Input;
using POS.Services.Login;
using POS.Utilities;
using POS.Utilities.RelayCommands;

namespace POS.ViewModels.StartFinishWork
{
    public class StartFinishWorkViewModel : ViewModelBase
    {
        private readonly SessionService _sessionService;

        private string employeeName;

        private bool isSessionNotActive = true;
        private bool isSessionActive = false;

        public string EmployeeName
        {
            get => employeeName;
            set => SetField(ref employeeName, value);
        }

        public bool IsSessionNotActive
        {
            get => isSessionNotActive;
            set => SetField(ref isSessionNotActive, value);
        }

        public bool IsSessionActive
        {
            get => isSessionActive;
            set => SetField(ref isSessionActive, value);
        }

        public ICommand StartSessionCommand { get; }
        public ICommand FinishSessionCommand { get; }

        public StartFinishWorkViewModel(SessionService sessionService)
        {
            _sessionService = sessionService;

            if(LoginManager.Instance.Employee != null)
                EmployeeName = LoginManager.Instance.Employee!.FirstName + " " + LoginManager.Instance.Employee.LastName;

            if (LoginManager.Instance.Employee!.IsUserLoggedIn)
            {
                IsSessionActive = !IsSessionActive;
                IsSessionNotActive = !IsSessionNotActive;
            }

            StartSessionCommand = new RelayCommandAsync(StartSession);
            FinishSessionCommand = new RelayCommandAsync(FinishSession);
        }

        private async Task StartSession()
        {
            var employee = LoginManager.Instance.Employee;
            await _sessionService.StartSession(employee!);
        }

        private async Task FinishSession()
        {
            var employee = LoginManager.Instance.Employee;
            await _sessionService.FinishSession(employee!);
        }
    }
}
