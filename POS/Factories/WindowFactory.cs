using POS.Views.Windows.SalesPanel;
using POS.Views.Windows.WarehouseFunctions;
using POS.Views.Windows;
using System;
using System.Collections.Generic;

namespace POS.Factories
{
    public class WindowFactory : ViewFactory
    {
        public WindowFactory()
        {
            views = new Dictionary<Type, Func<object>>
            {
                { typeof(MainWindow), () => new MainWindow() },
                { typeof(SalesPanelWindow), () => new SalesPanelWindow()},
                { typeof(ProductManagementWindow), () => new ProductManagementWindow() },
                { typeof(StockAndDeliveryManagementWindow), () => new StockAndDeliveryManagementWindow() },
            };
        }
    }
}
