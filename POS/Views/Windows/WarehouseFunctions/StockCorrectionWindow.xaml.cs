using DataAccess.Models;
using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.WarehouseFunctions;
using POS.Views.Base;

namespace POS.Views.Windows.WarehouseFunctions
{
    /// <summary>
    /// Interaction logic for StockCorrectionWindow.xaml
    /// </summary>
    public partial class StockCorrectionWindow : WindowBase
    {
        public StockCorrectionWindow(Ingredient ingredient)
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<StockCorrectionViewModel>();

            var viewModel = (StockCorrectionViewModel)DataContext;
            viewModel.CloseWindowBaseAction = Close;
            viewModel.SetSelectedIngredientCommand.Execute(ingredient);
            viewModel.LoadSelectedIngredientDataCommand.Execute(null);

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.DialogResult))
                    DialogResult = viewModel.DialogResult;
            };
        }
    }
}
