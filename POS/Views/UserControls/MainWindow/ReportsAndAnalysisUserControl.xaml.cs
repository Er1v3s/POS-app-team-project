using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.ReportsAndAnalysis;

namespace POS.Views.UserControls.MainWindow
{
    /// <summary>
    /// Logika interakcji dla klasy ReportsAndAnalysis.xaml
    /// </summary>
    public partial class ReportsAndAnalysisUserControl : UserControl
    {
        public ReportsAndAnalysisUserControl()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<ReportsAndAnalysisViewModel>();
        }
    }
}