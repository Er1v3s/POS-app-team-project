using Microsoft.Extensions.DependencyInjection;
using POS.Models.Orders;
using POS.ViewModels.SalesPanel;
using POS.Views.Base;

namespace POS.Views.Windows.SalesPanel
{
    /// <summary>
    /// Logika interakcji dla klasy OrderSummaryWindow.xaml
    /// </summary>
    public partial class OrderSummaryWindow : WindowBase
    {
        public OrderSummaryWindow(OrderDto orderDto)
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<OrderSummaryViewModel>();

            var viewModel = (OrderSummaryViewModel)DataContext;
            viewModel.CloseWindowBaseAction = Close;
            viewModel.OrderList = orderDto.OrderItemList;
            viewModel.AmountToPayForOrder = orderDto.AmountToPay;
            viewModel.Discount = orderDto.Discount;
            viewModel.InvoiceData = orderDto.InvoiceData;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.DialogResult))
                    DialogResult = viewModel.DialogResult;
            };
        }
    }
}

