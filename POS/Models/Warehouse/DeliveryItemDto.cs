using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DataAccess.Models;

namespace POS.Models.Warehouse
{
    public class DeliveryDto : INotifyPropertyChanged
    {
        private int quantity;

        public required Ingredient Ingredient { get; set; }

        public required int Quantity
        {
            get => quantity;
            set => SetField(ref quantity, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
