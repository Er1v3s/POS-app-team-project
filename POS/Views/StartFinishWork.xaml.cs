using POS.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace POS.Views
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
                var user = dbContext.Employees.FirstOrDefault(e => e.Employee_id == employeeId);
                if (user != null)
                {
                    employeeName.Text = user.First_name + " " + user.Last_name;
                    if(user.Is_User_LoggedIn)
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
                var user = dbContext.Employees.FirstOrDefault(e => e.Employee_id == employeeId);
                if (user != null)
                {
                    user.Is_User_LoggedIn = true;
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
                var user = dbContext.Employees.FirstOrDefault(e => e.Employee_id == employeeId);
                var employeeWorkSession = dbContext.EmployeeWorkSession.FirstOrDefault(e => e.Employee_Id == user.Employee_id && user.Is_User_LoggedIn);
                if (user != null)
                {
                    employeeWorkSession.Working_Time_To = DateTime.Now.ToString("HH:mm");
                    user.Is_User_LoggedIn = false;
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
                    Employee_Name = user.First_name + " " + user.Last_name,
                    Employee_Id = user.Employee_id,
                    Working_Time_From = DateTime.Now.ToString("HH:mm"),
                    Working_Time_To = null,
                    Working_Time_Summary = null,
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
