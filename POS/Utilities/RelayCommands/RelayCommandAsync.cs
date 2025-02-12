using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace POS.Utilities.RelayCommands
{
    public class RelayCommandAsync : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommandAsync(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public async void Execute(object parameter) => await _execute();

        public event EventHandler CanExecuteChanged;
    }

    public class RelayCommandAsync<T> : ICommand
    {
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommandAsync(Func<T, Task> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public async void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    await _execute((T)parameter);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during command execution: {ex.Message}");
                }
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
