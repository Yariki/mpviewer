using System;
using System.Windows.Input;

namespace MPViewer.Helpers
{
    public class MPCommand : ICommand
    {

        private Action<object> objectAction;
        private Func<object, bool> canExecuteAction;



        public MPCommand(Action<object> objectAction)
        {
            this.objectAction = objectAction;
        }


        public MPCommand(Action<object> objectAction, Func<object,bool> canExecuteAction)
            :this(objectAction)
        {
            this.canExecuteAction = canExecuteAction;
        }


        private event EventHandler CanExecuteChanged;

        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
                this.CanExecuteChanged += value;
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                this.CanExecuteChanged -= value;
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return canExecuteAction?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            objectAction?.Invoke(parameter);
        }
    }
}