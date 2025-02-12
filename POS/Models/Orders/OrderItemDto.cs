using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POS.Models.Orders
{
    public class OrderItemDto : INotifyPropertyChanged
    {
        private int amount;

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int RecipeId { get; set; }
        public double Price { get; set; }

        public int Amount
        {
            get => amount;
            set => SetField(ref amount, value);
        }

        public double TotalPrice => Price * Amount;

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
