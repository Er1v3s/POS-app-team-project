using System.Collections.Generic;
using System.Threading.Tasks;
using POS.Models.Orders;
using POS.Services.SalesPanel;
using POS.ViewModels.Base;

namespace POS.ViewModels.SalesPanel
{
    public class FinishedOrderViewModel : ViewModelBase
    {
        private readonly FinishedOrderService _finishedOrderService;

        private List<OrderHistoryDto> finishedOrders;

        public List<OrderHistoryDto> FinishedOrders
        {
            get => finishedOrders;
            set => SetField(ref finishedOrders, value);
        }

        public FinishedOrderViewModel(FinishedOrderService finishedOrderService)
        {
            _finishedOrderService = finishedOrderService;

            Task.Run(GenerateOrderHistory);
        }

        private async Task GenerateOrderHistory()
        {
            var orderHistory =  await _finishedOrderService.GetFinishedOrders();

            FinishedOrders = orderHistory;
        }
    }
}
