using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.AdminFunctionsPanel;
using POS.Views.Base;   

namespace POS.Views.Windows.AdminFunctionsPanel
{
    /// <summary>
    /// Logika interakcji dla klasy AddEditEmployeeWindow.xaml
    /// </summary>
    public partial class AddEmployeeWindow : FormInputWindow
    {
        public AddEmployeeWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<AddEmployeeViewModel>();

            var viewModel = (AddEmployeeViewModel)DataContext;
            viewModel.CloseWindowAction = this.Close;
        }
    }
}
