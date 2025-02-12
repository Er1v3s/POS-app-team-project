using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.StartFinishWork;

namespace POS.Views.UserControls.LoginPanelWindow
{
    /// <summary>
    /// Logika interakcji dla klasy StartFinishWork.xaml
    /// </summary>
    public partial class StartFinishWorkUserControl : UserControl
    {
        public StartFinishWorkUserControl()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<StartFinishWorkViewModel>();
        }
    }
}
