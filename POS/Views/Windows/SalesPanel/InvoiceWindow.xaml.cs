using Microsoft.Extensions.DependencyInjection;
using POS.ViewModels.SalesPanel;
using POS.Views.Base;

namespace POS.Views.Windows.SalesPanel
{
    /// <summary>
    /// Logika interakcji dla klasy InvoiceWindow.xaml
    /// </summary>
    public partial class InvoiceWindow : FormInputWindow
    {
        public InvoiceWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<InvoiceViewModel>();

            var viewModel = (InvoiceViewModel)DataContext;
            viewModel.CloseWindowBaseAction = Close;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.DialogResult))
                    DialogResult = viewModel.DialogResult;
            };
        }
    }
}
