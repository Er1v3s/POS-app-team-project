using System.Threading.Tasks;
using System.Windows.Input;
using POS.Services.Login;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

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

            StartSessionCommand = new RelayCommandAsync(StartSessionAsync);
            FinishSessionCommand = new RelayCommandAsync(FinishSessionAsync);
        }

        private async Task StartSessionAsync()
        {
            var employee = LoginManager.Instance.Employee;
            await _sessionService.StartSessionAsync(employee!);
        }

        private async Task FinishSessionAsync()
        {
            var employee = LoginManager.Instance.Employee;
            await _sessionService.FinishSessionAsync(employee!);
        }
    }
}
