using System.Collections.Generic;
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
        public OrderSummaryWindow(List<OrderItemDto> orderList, double amountToPayForOrder, int discount)
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<OrderSummaryViewModel>();

            var viewModel = (OrderSummaryViewModel)DataContext;
            viewModel.CloseWindowBaseAction = Close;
            viewModel.OrderList = orderList;
            viewModel.AmountToPayForOrder = amountToPayForOrder;
            viewModel.Discount = discount;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.DialogResult))
                    DialogResult = viewModel.DialogResult;
            };
        }
    }
}

