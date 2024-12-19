using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.ReportsAndAnalysis;

namespace POS.Views.ReportsAndAnalysisPanel
{
    /// <summary>
    /// Logika interakcji dla klasy ReportsAndAnalysis.xaml
    /// </summary>
    public partial class ReportsAndAnalysis : UserControl
    {
        public ReportsAndAnalysis()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<ReportsAndAnalysisViewModel>();
        }
    }
}