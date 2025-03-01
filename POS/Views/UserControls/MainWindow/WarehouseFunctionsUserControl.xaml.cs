using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.WarehouseFunctions;

namespace POS.Views.UserControls.MainWindow
{
    /// <summary>
    /// Logika interakcji dla klasy RunningOutOfIngredients.xaml
    /// </summary>
    public partial class WarehouseFunctionsUserControl : UserControl
    {
        public WarehouseFunctionsUserControl()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<WarehouseFunctionsViewModel>();


            var viewModel = (WarehouseFunctionsViewModel)DataContext;
            viewModel.LoadRunningOutOfIngredientsCommand.Execute(null);
        }
    }
}
