using System;
using System.Windows.Input;

namespace MASReportTool
{
    internal class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action<object> saveFileExecute, Func<bool> canSaveFileExecute)
        {
            _execute = saveFileExecute;
            _canExecute = canSaveFileExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}