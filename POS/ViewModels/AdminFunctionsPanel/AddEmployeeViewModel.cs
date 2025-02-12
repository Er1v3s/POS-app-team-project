using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataAccess.Models;
using POS.Validators;
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
                FirstName = FormValidator.ValidateString(firstName),
                LastName = FormValidator.ValidateString(lastName),
                JobTitle = FormValidator.ValidateString(jobTitle),
                Email = FormValidator.ValidateEmailAddress(email),
                PhoneNumber = ParsePhoneNumber(FormValidator.ValidatePhoneNumber(phoneNumber)),
                Address = FormValidator.ValidateString(address),
                Login = FormValidator.ValidateString(login),
                Password = FormValidator.ValidateString(password),
                IsUserLoggedIn = false
            };
        }
    }
}
