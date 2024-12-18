using Microsoft.Extensions.DependencyInjection;
using POS.Utilities;
using POS.ViewModels.AdminFunctionsPanel;

namespace POS.Views.AdminFunctionsPanel
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
