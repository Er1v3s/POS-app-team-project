using POS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POS.Views
{
    /// <summary>
    /// Logika interakcji dla klasy WorkTimeSummaryControl.xaml
    /// </summary>
    public partial class WorkTimeSummaryControl : UserControl
    {
        ObservableCollection<EmployeeWorkSession> ActiveSessions = new ObservableCollection<EmployeeWorkSession>();
        public WorkTimeSummaryControl()
        {
            InitializeComponent();
            ShowActiveSessions();

            StartFinishWork.WorkSessionChangeStatus += EmployeeWorkSession_SessionCreated;
        }

        private void ShowActiveSessions()
        {
            ActiveSessions.Clear();
            using (var dbContext = new AppDbContext())
            {
                var employeeWorkSession = dbContext.EmployeeWorkSession.ToList();

                if (employeeWorkSession != null)
                {
                    foreach (var session in employeeWorkSession)
                    {
                        var user = dbContext.Employees.FirstOrDefault(e => e.Employee_id == session.Employee_Id);
                        if (user != null)
                        {
                            DateTime workingTimeFrom = DateTime.ParseExact(session.Working_Time_From, "HH:mm", CultureInfo.InvariantCulture);
                            DateTime workingTimeTo;

                            if (session.Working_Time_To == "" || session.Working_Time_To == null)
                            {
                                workingTimeTo = DateTime.Now;
                            } 
                            else
                            {
                                workingTimeTo = DateTime.ParseExact(session.Working_Time_To, "HH:mm", CultureInfo.InvariantCulture);
                            }

                            TimeSpan workingTimeDifference = (workingTimeTo - workingTimeFrom);
                            byte hours = (byte)workingTimeDifference.TotalHours;
                            byte minutes = (byte)workingTimeDifference.Minutes;
                            string formattedTimeDifference = $"{hours:D2}:{minutes:D2}";

                            session.Working_Time_Summary = formattedTimeDifference;
                            dbContext.SaveChangesAsync();
                            ActiveSessions.Add(session);
                        }
                    }
                }
            }

            workingTimeSummaryDataGrid.ItemsSource = ActiveSessions;
        }

        private void Refresh_ButtonClick(object sender, RoutedEventArgs e)
        {
            ShowActiveSessions();
        }

        private void EmployeeWorkSession_SessionCreated(object sender, EventArgs e)
        {
            ShowActiveSessions();
        }
    }
}
