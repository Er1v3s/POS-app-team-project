using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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

        public async Task AddRangeWithDelay(IEnumerable<T> items, int delay)
        {
            if (items == null)
                throw new ArgumentNullException($"Niepoprawna kolekcja: {items}");

            foreach (var item in items)
            {
                await Task.Delay(delay);
                this.Add(item);
            }
        }
    }
}
