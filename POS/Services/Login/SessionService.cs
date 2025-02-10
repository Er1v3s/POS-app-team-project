using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using POS.Utilities;

namespace POS.Services.Login
{
    public class SessionService
    {
        private readonly AppDbContext _dbContext;

        public MyObservableCollection<EmployeeWorkSession> SessionCollection { get; }

        public SessionService(AppDbContext dbContext)
        {
            _dbContext = dbContext;

            SessionCollection = new();
            _ = GetSessionsAsync();
        }

        public async Task StartSessionAsync(Employee employee)
        {
            if (!employee.IsUserLoggedIn)
                await CreateNewSessionAsync(employee);
        }

        public async Task FinishSessionAsync(Employee employee)
        {
            if (employee.IsUserLoggedIn)
                await FinishExistedSessionAsync(employee);

            LoginManager.Instance.LogOut();
        }

        public async Task GetSessionsAsync()
        {
            var sessions = await GetAllSessionsFromDbAsync();
            var sessionsWithTimeInterval = SetTimeInterval(sessions);

            SessionCollection.Clear();
            await SessionCollection.AddRangeWithDelay(sessionsWithTimeInterval, 50);
        }

        public async Task<bool> CheckForActiveSessionsAsync()
        {
            var sessions = await _dbContext.EmployeeWorkSession.FirstOrDefaultAsync(s => s.WorkingTimeTo == null);

            return LoginManager.Instance.IsAnySessionActive = sessions != null;
        }

        private async Task CreateNewSessionAsync(Employee employee)
        {
            employee.IsUserLoggedIn = true;
            LoginManager.Instance.IsAnySessionActive = true;

            var employeeWorkSession = new EmployeeWorkSession
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.FirstName + " " + employee.LastName,
                WorkingTimeFrom = DateTime.Now.ToString("HH:mm"),
                WorkingTimeTo = null,
                WorkingTimeSummary = null,
            };

            await _dbContext.EmployeeWorkSession.AddAsync(employeeWorkSession);
            await _dbContext.SaveChangesAsync();
        }

        private async Task FinishExistedSessionAsync(Employee employee)
        {
            var employeeWorkSession = await _dbContext.EmployeeWorkSession
                    .Where(e => e.WorkingTimeTo == null)
                    .FirstOrDefaultAsync(e => e.EmployeeId == employee.EmployeeId && employee.IsUserLoggedIn);

            if (employeeWorkSession != null)
            {
                employeeWorkSession.WorkingTimeTo = DateTime.Now.ToString("HH:mm");
                employee.IsUserLoggedIn = false;
                await _dbContext.SaveChangesAsync();
            }

            await CheckForActiveSessionsAsync();
        }

        private async Task<List<EmployeeWorkSession>> GetAllSessionsFromDbAsync()
        {
            var sessions = await _dbContext.EmployeeWorkSession.ToListAsync();

            return sessions;
        }

        private List<EmployeeWorkSession> SetTimeInterval(List<EmployeeWorkSession> sessions)
        {
            List<EmployeeWorkSession> sessionsWithTimeInterval = new();

            foreach (var session in sessions)
            {
                DateTime workingTimeFrom =
                    DateTime.ParseExact(session.WorkingTimeFrom, "HH:mm", CultureInfo.InvariantCulture);
                DateTime workingTimeTo =
                    string.IsNullOrEmpty(session.WorkingTimeTo)
                        ? DateTime.Now
                        : DateTime.ParseExact(session.WorkingTimeTo, "HH:mm", CultureInfo.InvariantCulture);


                TimeSpan workingTimeDifference = workingTimeTo - workingTimeFrom;
                session.WorkingTimeSummary =
                    $"{(int)workingTimeDifference.TotalHours:D2}:{workingTimeDifference.Minutes:D2}";

                sessionsWithTimeInterval.Add(session);
            }

            return sessionsWithTimeInterval;
        }
    }
}
