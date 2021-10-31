using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DoktersApplicatie.Commands
{
    public abstract class AsyncCommandBase : CommandBase
    {
        private readonly Action<Exception> _OnException;

        private bool _isExecuting;
        public bool IsExecuting
        {
            get
            {
                return _isExecuting;
            }
            set
            {
                _isExecuting = value;
                OnCanExecuteChanged();
            }
        }

        public AsyncCommandBase(Action<Exception> onException)
        {
            _OnException = onException;
        }

        public override bool CanExecute(object parameter)
        {
            return !IsExecuting;
        }

        public override async void Execute(object parameter)
        {
            IsExecuting = true;

            try
            {
                await ExecuteAsync(parameter);
            }
            catch (Exception e)
            {
                _OnException?.Invoke(e);
            }

            IsExecuting = false;
        }

        protected abstract Task ExecuteAsync(object parameter);
    }
}
