using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using POS.Helpers;
using POS.Services.AdminFunctions;
using POS.Utilities.RelayCommands;

namespace POS.ViewModels.AdminFunctionsPanel
{
    public class AddEmployeeViewModel : EmployeeViewModelBase
    {
        private readonly AdminFunctionsService _adminFunctionsService;
        public ICommand AddEmployeeCommand { get; }
        
        public AddEmployeeViewModel(AdminFunctionsService adminFunctionsService)
        {
            _adminFunctionsService = adminFunctionsService;

            AddEmployeeCommand = new RelayCommandAsync(AddEmployee);
        }

        private async Task AddEmployee()
        {
            try
            {
                var newEmployee = CreateEmployee();
                await _adminFunctionsService.AddEmployeeAsync(newEmployee);
                CloseWindowAction?.Invoke();

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Employee CreateEmployee()
        {
            return new Employee
            {
                FirstName = FormValidatorHelper.ValidateString(firstName),
                LastName = FormValidatorHelper.ValidateString(lastName),
                JobTitle = FormValidatorHelper.ValidateString(jobTitle),
                Email = FormValidatorHelper.ValidateEmailAddress(email),
                PhoneNumber = ParsePhoneNumber(FormValidatorHelper.ValidatePhoneNumber(phoneNumber)),
                Address = FormValidatorHelper.ValidateString(address),
                Login = FormValidatorHelper.ValidateString(login),
                Password = FormValidatorHelper.ValidateString(password),
                IsUserLoggedIn = false
            };
        }
    }
}
