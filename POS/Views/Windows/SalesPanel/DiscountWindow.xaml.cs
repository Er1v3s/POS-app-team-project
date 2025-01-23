using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.SalesPanel;
using POS.Views.Base;

namespace POS.Views.Windows.SalesPanel
{
    /// <summary>
    /// Logika interakcji dla klasy DiscountWindow.xaml
    /// </summary>
    public partial class DiscountWindow : WindowBase
    {
        public DiscountWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<DiscountWindowViewModel>();

            var viewModel = (DiscountWindowViewModel)DataContext;
            viewModel.CloseWindowBaseAction = Close;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.DialogResult))
                    DialogResult = viewModel.DialogResult;
            };
        }
    }
}