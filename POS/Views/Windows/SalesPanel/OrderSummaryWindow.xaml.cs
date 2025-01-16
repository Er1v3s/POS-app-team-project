using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using POS.Models.Orders;
using POS.ViewModels.SalesPanel;

namespace POS.Views.Windows.SalesPanel
{
    /// <summary>
    /// Logika interakcji dla klasy OrderSummaryWindow.xaml
    /// </summary>
    public partial class OrderSummaryWindow : Window
    {
        public OrderSummaryWindow(List<OrderItemDto> orderList, double amountToPayForOrder)
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<OrderSummaryViewModel>();

            var viewModel = (OrderSummaryViewModel)DataContext;
            viewModel.CloseWindowAction = Close;
            viewModel.OrderList = orderList;
            viewModel.AmountToPayForOrder = amountToPayForOrder;

            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(viewModel.DialogResult))
                    DialogResult = viewModel.DialogResult;
            };
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void CloseWindow_ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

