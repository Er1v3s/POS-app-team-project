using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using POS.Models.AdminFunctions;

namespace POS.Services.AdminFunctions
{
    public class AdminFunctionsService
    {
        private readonly AppDbContext _dbContext;
        public AdminFunctionsService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Collection<EmployeeInfoDto>> LoadEmployeeInfoList()
        {
            var employeesInfoList = new Collection<EmployeeInfoDto>();

            var employees = await _dbContext.Employees.ToListAsync();

            foreach (var employee in employees)
            {
                var employeeInfo = new EmployeeInfoDto()
                {
                    EmployeeId = employee.EmployeeId,
                    EmployeeName = employee.FirstName + " " + employee.LastName,
                    JobTitle = employee.JobTitle ?? "",
                    PermissionLevel = 5 // temporary data
                };

                employeesInfoList.Add(employeeInfo);

            }

            return employeesInfoList;
        }

        public async Task<Employees> LoadEmployeeData(EmployeeInfoDto selectedEmployeeInfo)
        {
            return (await _dbContext.Employees.FirstOrDefaultAsync(employee => employee.EmployeeId == selectedEmployeeInfo.EmployeeId))!;
        }

        public async Task AddEmployee(Employees newEmployee)
        {
            await _dbContext.Employees.AddAsync(newEmployee);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EditEmployee(Employees selectedEmployee)
        {
            var employeeToUpdate = await _dbContext.Employees.FirstOrDefaultAsync(employee => employee.EmployeeId == selectedEmployee.EmployeeId);

            if (employeeToUpdate != null)
            {
                employeeToUpdate.FirstName = selectedEmployee.FirstName;
                employeeToUpdate.LastName = selectedEmployee.LastName;
                employeeToUpdate.JobTitle = selectedEmployee.JobTitle;
                employeeToUpdate.Email = selectedEmployee.Email;
                employeeToUpdate.PhoneNumber = selectedEmployee.PhoneNumber;
                employeeToUpdate.Address = selectedEmployee.Address;
                employeeToUpdate.Login = selectedEmployee.Login;
                employeeToUpdate.Password = selectedEmployee.Password;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteEmployee(EmployeeInfoDto selectedEmployeeInfo)
        {
            var employeeToRemove = await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == selectedEmployeeInfo.EmployeeId);

            if (employeeToRemove != null)
            {
                _dbContext.Employees.Remove(employeeToRemove);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<double> ShowCashBalance()
        {
            var cashBalance = await _dbContext.Payments
                                .Where(payment => payment.PaymentMethod == "Gotówka")
                                .SumAsync(payment => payment.Amount);
            
            return Math.Round(cashBalance, 2);
        }
    }
}
