using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.WorkTimeSummaryControl;

namespace POS.Views.UserControls.MainWindow
{
    /// <summary>
    /// Logika interakcji dla klasy WorkTimeSummaryControl.xaml
    /// </summary>
    public partial class WorkTimeSummaryUserControl : UserControl
    {
        public WorkTimeSummaryUserControl()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<WorkTimeSummaryControlViewModel>();
        }
    }
}
