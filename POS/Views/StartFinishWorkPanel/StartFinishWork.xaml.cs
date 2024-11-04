using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DataAccess.Models;

namespace POS.Views.StartFinishWorkPanel
{
    /// <summary>
    /// Logika interakcji dla klasy StartFinishWork.xaml
    /// </summary>
    public partial class StartFinishWork : UserControl
    {
        private readonly int employeeId;
        public static event EventHandler<EventArgs>? WorkSessionChangeStatus;

        public StartFinishWork(int employeeId)
        {
            this.employeeId = employeeId;
            InitializeComponent();

            using (var dbContext = new AppDbContext())
            {
                var user = dbContext.Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
                if (user != null)
                {
                    employeeName.Text = user.FirstName + " " + user.LastName;
                    if(user.IsUserLoggedIn)
                    {
                        StartWork.IsEnabled = false;
                    }
                    else
                    {
                        FinishWork.IsEnabled = false;
                    }
                }
            }
        }

        private async void StartWork_ButtonClick(object sender, RoutedEventArgs e)
        {
            await using (var dbContext = new AppDbContext())
            {
                var user = dbContext.Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
                if (user != null)
                {
                    user.IsUserLoggedIn = true;
                    dbContext.SaveChanges();
                }

                EmployeeWorkSession newEmployeeWorkSession = CreateNewEmployeeWorkSession(user);

                dbContext.EmployeeWorkSession.Add(newEmployeeWorkSession);
                dbContext.SaveChanges();
            }

            OnStartFinishWork();
        }

        private async void FinishWork_ButtonClick(object sender, RoutedEventArgs e)
        {
            await using (var dbContext = new AppDbContext())
            {
                var user = dbContext.Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
                var employeeWorkSession = dbContext.EmployeeWorkSession.FirstOrDefault(e => e.EmployeeId == user.EmployeeId && user.IsUserLoggedIn);
                if (user != null)
                {
                    employeeWorkSession.WorkingTimeTo = DateTime.Now.ToString("HH:mm");
                    user.IsUserLoggedIn = false;
                    dbContext.SaveChanges();
                }
            }

            OnStartFinishWork();
        }

        private EmployeeWorkSession CreateNewEmployeeWorkSession(Employees user)
        {
            EmployeeWorkSession employeeWorkSession = (
                new EmployeeWorkSession
                {
                    EmployeeName = user.FirstName + " " + user.LastName,
                    EmployeeId = user.EmployeeId,
                    WorkingTimeFrom = DateTime.Now.ToString("HH:mm"),
                    WorkingTimeTo = null,
                    WorkingTimeSummary = null,
                }
            );

            return employeeWorkSession;
        }

        protected virtual void OnStartFinishWork()
        {
            WorkSessionChangeStatus?.Invoke(this, EventArgs.Empty);
        }
    }
}
