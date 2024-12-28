using System;
using System.Windows.Input;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.AdminFunctionsPanel
{
    public abstract class EmployeeViewModelBase : ViewModelBase
    {
        protected string firstName;
        protected string lastName;
        protected string jobTitle;
        protected string email;
        protected string phoneNumber;
        protected string address;
        protected string login;
        protected string password;

        public string FirstName
        {
            get => firstName;
            set => SetField(ref firstName, value);
        }

        public string LastName
        {
            get => lastName;
            set => SetField(ref lastName, value);
        }

        public string JobTitle
        {
            get => jobTitle;
            set => SetField(ref jobTitle, value);
        }

        public string Email
        {
            get => email;
            set => SetField(ref email, value);
        }

        public string PhoneNumber
        {
            get => phoneNumber;
            set => SetField(ref phoneNumber, value);
        }

        public string Address
        {
            get => address;
            set => SetField(ref address, value);
        }

        public string Login
        {
            get => login;
            set => SetField(ref login, value);
        }

        public string Password
        {
            get => password;
            set => SetField(ref password, value);
        }

        public Action CloseWindowAction { get; set; }
        public ICommand CloseWindowCommand { get; set; }

        protected EmployeeViewModelBase()
        {
            CloseWindowCommand = new RelayCommand(CloseWindow);
        }

        protected virtual int ParsePhoneNumber(string txtPhoneNumber)
        {
            if (int.TryParse(txtPhoneNumber, out var intPhoneNumber))
                return intPhoneNumber;
            else
                intPhoneNumber = 000000000;
            
            return intPhoneNumber;
        }

        protected void CloseWindow(object obj)
        {
            CloseWindowAction?.Invoke();
        }
    }
}
