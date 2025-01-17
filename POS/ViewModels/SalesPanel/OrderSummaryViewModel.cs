using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using POS.Models.Orders;
using POS.Services.SalesPanel;
using POS.Utilities.RelayCommands;
using POS.ViewModels.Base;

namespace POS.ViewModels.SalesPanel
{
    public class OrderSummaryViewModel : ViewModelBase
    {
        private bool dialogResult;

        private List<OrderItemDto> orderList;
        private double amountToPayForOrder;
        private int discount;

        private readonly OrderSummaryService _orderSummaryService;

        public bool DialogResult
        {
            get => dialogResult;
            set => SetField(ref dialogResult, value);
        }

        public List<OrderItemDto> OrderList
        {
            get => orderList;
            set => SetField(ref orderList, value);
        }

        public double AmountToPayForOrder
        {
            get => amountToPayForOrder;
            set => SetField(ref amountToPayForOrder, Math.Round(value, 2));
        }

        public int Discount
        {
            get => discount;
            set => SetField(ref discount, value);
        }

        public ICommand FinishOrderCommand { get; }

        public OrderSummaryViewModel(OrderSummaryService orderSummaryService)
        {
            _orderSummaryService = orderSummaryService;

            FinishOrderCommand = new RelayCommandAsync(FinishOrder);
        }

        private async Task FinishOrder()
        { 
            var result = await _orderSummaryService.GenerateBill(orderList, amountToPayForOrder, discount);

            if(result)
                DialogResult = true;

            CloseWindowBaseAction!.Invoke();
        }
    }
}
