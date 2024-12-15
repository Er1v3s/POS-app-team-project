using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.StartFinishWork;

namespace POS.Views.StartFinishWorkPanel
{
    /// <summary>
    /// Logika interakcji dla klasy StartFinishWork.xaml
    /// </summary>
    public partial class StartFinishWork : UserControl
    {
        public StartFinishWork()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<StartFinishWorkViewModel>();
        }
    }
}
