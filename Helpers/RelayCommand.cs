using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PerpetuaNet.Helpers
{
    public class RelayCommand : ICommand, INotifyPropertyChanged
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
            RaiseCanExecuteChanged();
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
