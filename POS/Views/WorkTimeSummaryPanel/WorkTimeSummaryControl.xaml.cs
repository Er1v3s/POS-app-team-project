using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.WorkTimeSummaryControl;

namespace POS.Views.WorkTimeSummaryPanel
{
    /// <summary>
    /// Logika interakcji dla klasy WorkTimeSummaryControl.xaml
    /// </summary>
    public partial class WorkTimeSummaryControl : UserControl
    {
        public WorkTimeSummaryControl()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<WorkTimeSummaryControlViewModel>();
        }

        private void Refresh_ButtonClick(object sender, RoutedEventArgs e)
        {
            var workTimeSummaryControlViewModel = (WorkTimeSummaryControlViewModel)DataContext;
            workTimeSummaryControlViewModel.RefreshCommand.Execute(null);
        }
    }
}
