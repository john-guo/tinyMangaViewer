using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace tinyMangaViewer
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> _action;
        private Func<object, bool> _func;

        public RelayCommand(Action<object> action, Func<object, bool> func = null)
        {
            _action = action;
            _func = func;
        }

        public bool CanExecute(object parameter)
        {
            return _func?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke(parameter);
        }

        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class RelayCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<T> _action;
        private Func<T, bool> _func;

        public RelayCommand(Action<T> action, Func<T, bool> func = null)
        {
            _action = action;
            _func = func;
        }

        public bool CanExecute(object parameter)
        {
            return _func?.Invoke((T)parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke((T)parameter);
        }

        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
