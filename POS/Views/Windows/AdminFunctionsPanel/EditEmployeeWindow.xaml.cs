using Microsoft.Extensions.DependencyInjection;
using POS.Models.AdminFunctions;
using POS.ViewModels.AdminFunctionsPanel;
using POS.Views.Base;


namespace POS.Views.Windows.AdminFunctionsPanel
{
    /// <summary>
    /// Logika interakcji dla klasy EditEmployeeWindow.xaml
    /// </summary>
    public partial class EditEmployeeWindow : FormInputWindow
    {
        public EditEmployeeWindow(EmployeeInfoDto selectedEmployee)
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<EditEmployeeViewModel>();

            var viewModel = (EditEmployeeViewModel)DataContext;
            viewModel.SetSelectedEmployeeCommand.Execute(selectedEmployee);
            viewModel.LoadSelectedEmployeeDataCommand.Execute(null);
            viewModel.CloseWindowAction = this.Close;
        }
    }
}
