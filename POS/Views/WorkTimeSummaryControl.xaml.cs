using POS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                        var user = dbContext.Employees.FirstOrDefault(e => e.Employee_id == session.Employee_Id && e.Is_User_LoggedIn);
                        if (user != null)
                        {
                            ActiveSessions.Add(session);
                        }
                    }
                }
            }

            workingTimeSummaryDataGrid.ItemsSource = ActiveSessions;
        }

        private void Refresh_Button(object sender, RoutedEventArgs e)
        {
            ShowActiveSessions();
        }

        private void EmployeeWorkSession_SessionCreated(object sender, EventArgs e)
        {
            ShowActiveSessions();
        }
    }
}
