using System;

namespace POS.Factories
{
    public interface IViewFactory
    {
        object GetView(Type viewType);
    }
}
