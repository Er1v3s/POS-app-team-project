using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using POS.Views.StartFinishWorkPanel;

namespace POS.Views.WorkTimeSummaryPanel
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

        private async Task ShowActiveSessions()
        {
            ActiveSessions.Clear();
            await using (var dbContext = new AppDbContext())
            {
                var employeeWorkSession = dbContext.EmployeeWorkSession.ToList();

                if (employeeWorkSession != null)
                {
                    foreach (var session in employeeWorkSession)
                    {
                        var user = await dbContext.Employees.FirstOrDefaultAsync(e => e.EmployeeId == session.EmployeeId);
                        if (user != null)
                        {
                            DateTime workingTimeFrom = DateTime.ParseExact(session.WorkingTimeFrom, "HH:mm", CultureInfo.InvariantCulture);
                            DateTime workingTimeTo;

                            if (string.IsNullOrEmpty(session.WorkingTimeTo))
                            {
                                workingTimeTo = DateTime.Now;
                            } 
                            else
                            {
                                workingTimeTo = DateTime.ParseExact(session.WorkingTimeTo, "HH:mm", CultureInfo.InvariantCulture);
                            }

                            TimeSpan workingTimeDifference = (workingTimeTo - workingTimeFrom);
                            byte hours = (byte)workingTimeDifference.TotalHours;
                            byte minutes = (byte)workingTimeDifference.Minutes;
                            string formattedTimeDifference = $"{hours:D2}:{minutes:D2}";

                            session.WorkingTimeSummary = formattedTimeDifference;
                            await dbContext.SaveChangesAsync();
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
