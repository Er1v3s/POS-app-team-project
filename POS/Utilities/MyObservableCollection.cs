using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace POS.Utilities
{
    public class MyObservableCollection<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException($"Niepoprawna kolekcja: {items}");

            foreach (var item in items)
                this.Add(item);
        }
    }
}
