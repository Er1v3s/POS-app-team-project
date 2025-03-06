using System;
using System.Collections.Generic;
using POS.Views.UserControls.MainWindow;

namespace POS.Factories
{
    public class UserControlFactory : ViewFactory
    {
        public UserControlFactory()
        {
            views = new Dictionary<Type, Func<object>>
            {
                { typeof(WorkTimeSummaryUserControl), () => new WorkTimeSummaryUserControl() },
                { typeof(WarehouseFunctionsUserControl), () => new WarehouseFunctionsUserControl() },
                { typeof(ReportsAndAnalysisUserControl), () => new ReportsAndAnalysisUserControl() },
                { typeof(AdminFunctionsUserControl), () => new AdminFunctionsUserControl() }
            };
        }
    }
}
