using System;
using System.Collections.Generic;

namespace POS.Factories
{
    public class ViewFactory : IViewFactory
    {
        protected Dictionary<Type, Func<object>> views;

        public object GetView(Type viewType)
        {
            views.TryGetValue(viewType, out var value);
            if (value != null)
                return value();

            throw new Exception("Nie udało się poprawnie załadować widoku");
        }
    }
}
