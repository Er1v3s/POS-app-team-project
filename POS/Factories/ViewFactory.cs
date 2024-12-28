using System;
using System.Collections.Generic;
using POS.Views.UserControls.MainWindow;

namespace POS.Factories
{
    public class ViewFactory
    {
        private readonly Dictionary<int, Func<object>> viewFactoryDictionary;

        public ViewFactory()
        {
            viewFactoryDictionary = new Dictionary<int, Func<object>>
            {
                { 0, () => new WorkTimeSummaryUserControl() },
                { 1, () => new WarehouseFunctionsUserControl() },
                { 2, () => new ReportsAndAnalysisUserControl() },
                { 3, () => new AdminFunctionsUserControl() }
            };
        }

        public object GetView(int parameter)
        {
            viewFactoryDictionary.TryGetValue(parameter, out var value);
            if (value != null)
                return value();

            throw new Exception("Nie udało się poprawnie załadować widoku");
        }
    }
}
