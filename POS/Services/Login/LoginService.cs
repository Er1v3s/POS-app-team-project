using System;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace POS.Services.Login
{
    public class LoginService
    {
        private readonly AppDbContext _dbContext;
        public LoginService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AuthenticateUser(string login, string password)
        {
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Login == login && e.Password == password);

            if (employee != null)
                LoginManager.Instance.LogIn(employee);
            else
            {
                LoginManager.Instance.LogOut();
                throw new Exception("Incorrect login or password");
            }
        }
    }
}
