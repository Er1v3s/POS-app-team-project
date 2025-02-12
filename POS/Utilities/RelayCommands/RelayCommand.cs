using System;
using System.Windows.Input;

namespace POS.Utilities.RelayCommands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _executeWithParam;
        private readonly Action _executeWithoutParam;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action executeWithoutParam, Func<bool> canExecute = null)
        {
            _executeWithoutParam = executeWithoutParam ?? throw new ArgumentNullException(nameof(executeWithoutParam));
            _canExecute = canExecute != null ? _ => canExecute() : null;
        }

        public RelayCommand(Action<object> executeWithParam, Func<object, bool> canExecute = null)
        {
            _executeWithParam = executeWithParam ?? throw new ArgumentNullException(nameof(executeWithParam));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object? parameter)
        {
            if (_executeWithParam != null)
                _executeWithParam(parameter);
            else
                _executeWithoutParam();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute((T)parameter);

        public void Execute(object? parameter) => _execute((T)parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
