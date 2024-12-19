using POS.Views.AdminFunctionsPanel;
using POS.Views.ReportsAndAnalysisPanel;
using POS.Views.WarehouseFunctionsPanel;
using POS.Views.WorkTimeSummaryPanel;
using System;
using System.Collections.Generic;

namespace POS.Factories
{
    public class ViewFactory
    {
        private readonly Dictionary<int, Func<object>> viewFactoryDictionary;

        public ViewFactory()
        {
            viewFactoryDictionary = new Dictionary<int, Func<object>>
            {
                { 0, () => new WorkTimeSummaryControl() },
                { 1, () => new RunningOutOfIngredients() },
                { 2, () => new ReportsAndAnalysis() },
                { 3, () => new AdministratorFunctions() }
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
