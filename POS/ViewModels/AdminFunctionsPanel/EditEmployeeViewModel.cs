using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using POS.Validators;
using POS.Models.AdminFunctions;
using POS.Services.AdminFunctions;
using POS.Utilities.RelayCommands;

namespace POS.ViewModels.AdminFunctionsPanel
{
    public class EditEmployeeViewModel : EmployeeViewModelBase
    {
        private readonly AdminFunctionsService _adminFunctionsService;

        private EmployeeInfoDto selectedEmployee;
        private Employee selectedEmployeeFullData;

        public ICommand EditEmployeeCommand { get; }
        public ICommand LoadSelectedEmployeeDataCommand;
        public ICommand SetSelectedEmployeeCommand;
        
        public EditEmployeeViewModel(AdminFunctionsService adminFunctionsService)
        {
            _adminFunctionsService = adminFunctionsService;

            EditEmployeeCommand = new RelayCommandAsync(EditEmployee);
            SetSelectedEmployeeCommand = new RelayCommand<EmployeeInfoDto>(SetSelectedEmployee);
            LoadSelectedEmployeeDataCommand = new RelayCommandAsync(LoadSelectedEmployeeData);
        }

        private async Task EditEmployee()
        {
            try
            {
                EditEmployeeData();
                await _adminFunctionsService.EditEmployeeAsync(selectedEmployeeFullData);
                CloseWindowAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditEmployeeData()
        {
            selectedEmployeeFullData.FirstName = FormValidator.ValidateString(firstName);
            selectedEmployeeFullData.LastName = FormValidator.ValidateString(lastName);
            selectedEmployeeFullData.JobTitle = FormValidator.ValidateString(jobTitle);
            selectedEmployeeFullData.Email = FormValidator.ValidateEmailAddress(email);
            selectedEmployeeFullData.PhoneNumber = ParsePhoneNumber(FormValidator.ValidatePhoneNumber(phoneNumber));
            selectedEmployeeFullData.Address = FormValidator.ValidateString(address);
            selectedEmployeeFullData.Login = FormValidator.ValidateString(login);
            selectedEmployeeFullData.Password = FormValidator.ValidateString(password);
        }

        private void SetSelectedEmployee(EmployeeInfoDto employee)
        {
            selectedEmployee = employee;
        }

        private async Task LoadSelectedEmployeeData()
        {
            var employee = await _adminFunctionsService.LoadEmployeeDataAsync(selectedEmployee);

            selectedEmployeeFullData = employee;
            SetLoadedEmployeeDataInForm(employee);
        }

        private void SetLoadedEmployeeDataInForm(Employee employeeToUpdate)
        {
            FirstName = employeeToUpdate.FirstName;
            LastName = employeeToUpdate.LastName;
            JobTitle = employeeToUpdate.JobTitle ?? "";
            Email = employeeToUpdate.Email ?? "";
            PhoneNumber = employeeToUpdate.PhoneNumber.ToString() ?? "";
            Address = employeeToUpdate.Address ?? " ";
            Login = employeeToUpdate.Login;
            Password = employeeToUpdate.Password;
        }
    }
}
