using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using POS.Utilities.RelayCommands;

namespace POS.ViewModels.Base
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        private bool isButtonEnable = true;

        public bool IsButtonEnable
        {
            get => isButtonEnable;
            set => SetField(ref isButtonEnable, value);
        }

        public Action? CloseWindowBaseAction;
        public ICommand CloseWindowBaseCommand => new RelayCommand(CloseWindowBaseAction!);

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            Debug.WriteLine($"Property changed: {propertyName}");
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
