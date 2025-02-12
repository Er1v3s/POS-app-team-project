using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using POS.Models.AdminFunctions;
using POS.Services.AdminFunctions;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;
using POS.Views.Windows.AdminFunctionsPanel;

namespace POS.ViewModels.AdminFunctionsPanel
{
    public class AdminFunctionsViewModel : ViewModelBase
    {
        private readonly AdminFunctionsService _adminFunctionsService;

        private ObservableCollection<EmployeeInfoDto> employeesCollection = [];

        private EmployeeInfoDto selectedEmployee;
        private bool isEditButtonEnabled;
        private bool isDeleteButtonEnabled;

        public bool IsEditButtonEnabled
        {
            get => isEditButtonEnabled;
            set => SetField(ref isEditButtonEnabled, value);
        }

        public bool IsDeleteButtonEnabled
        {
            get => isDeleteButtonEnabled;
            set => SetField(ref isDeleteButtonEnabled, value);
        }

        public EmployeeInfoDto SelectedEmployee
        {
            get => selectedEmployee;
            set
            {
                SetField(ref selectedEmployee, value);
                UpdateButtonStates();
            }
        }

        public ObservableCollection<EmployeeInfoDto> EmployeesCollection
        {
            get => employeesCollection;
            set => SetField(ref employeesCollection, value);
        }

        public ICommand LoadEmployeeInfoListCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        public ICommand EditEmployeeCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }
        public ICommand ShowCashBalanceCommand { get; }

        public AdminFunctionsViewModel(AdminFunctionsService adminFunctionsService)
        {
            _adminFunctionsService = adminFunctionsService;

            LoadEmployeeInfoListCommand = new RelayCommandAsync(LoadEmployeeInfoList);
            AddEmployeeCommand = new RelayCommandAsync(AddEmployee);
            EditEmployeeCommand = new RelayCommandAsync(EditEmployee);
            DeleteEmployeeCommand = new RelayCommandAsync(DeleteEmployee);
            ShowCashBalanceCommand = new RelayCommandAsync(ShowCashBalance);

            LoadEmployeeInfoListCommand.Execute(null);
        }

        private async Task LoadEmployeeInfoList()
        {
            employeesCollection.Clear();

            var employeeList = await _adminFunctionsService.LoadEmployeeInfoListAsync();

            foreach (var employee in employeeList)
                employeesCollection.Add(employee);
        }

        private async Task AddEmployee()
        {
            var addEmployeeWindow = new AddEmployeeWindow();
            addEmployeeWindow.ShowDialog();

            await LoadEmployeeInfoList();
        }

        private async Task EditEmployee()
        {
            var editEmployeeWindow = new EditEmployeeWindow(selectedEmployee);
            editEmployeeWindow.ShowDialog();

            await LoadEmployeeInfoList();
        }

        private async Task DeleteEmployee()
        {
            var result = MessageBox.Show($"Czy na pewno chcesz usunąć pracownika {selectedEmployee.EmployeeName}?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await _adminFunctionsService.DeleteEmployeeAsync(selectedEmployee);
                await LoadEmployeeInfoList();
            }
        }

        private async Task ShowCashBalance()
        {
            var cashBalance = await _adminFunctionsService.ShowCashBalanceAsync();

            MessageBox.Show($"Stan kasy wynosi: {cashBalance:C}");
        }

        private void UpdateButtonStates()
        {
            IsEditButtonEnabled = selectedEmployee != null;
            IsDeleteButtonEnabled = selectedEmployee != null;
        }
    }
}
