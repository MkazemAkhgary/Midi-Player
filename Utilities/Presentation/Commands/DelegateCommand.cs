using System;
using System.Threading.Tasks;
using System.Windows.Input;
using JetBrains.Annotations;

// ReSharper disable UnusedMember.Global

namespace Utilities.Presentation.Commands
{
    /// <summary>
    /// An <see cref="ICommand"/> whose delegates can be attached for Execute and CanExecute.
    /// </summary>
    public sealed class DelegateCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, Task> _executeAsync;

        private readonly Predicate<object> _canExecute;

        private DelegateCommand(Predicate<object> canExecute)
        {
            if (canExecute == null) throw new ArgumentNullException(nameof(canExecute));

            _canExecute = canExecute;
        }

        private DelegateCommand(Action<object> execute, Predicate<object> canExecute) : this(canExecute)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));

            _execute = execute;
            _executeAsync = o => Task.Run(() => execute(o));
        }

        private DelegateCommand(Func<object, Task> executeAsync, Predicate<object> canExecute) : this(canExecute)
        {
            if (executeAsync == null) throw new ArgumentNullException(nameof(executeAsync));

            _execute = o => executeAsync(o);
            _executeAsync = executeAsync;
        }

        /// <summary>
        /// creates new instance of <see cref="DelegateCommand"/>.
        /// </summary>
        /// <typeparam name="T">type of parameter passed to execute and canExecute delegates.</typeparam>
        /// <param name="execute">delegate that takes object of type <see cref="T"/> to execute command.</param>
        /// <param name="canExecute">delegate that takes object of type <see cref="T"/> to check whether command can be executed or not. can be null if command should be always executed.</param>
        [NotNull]
        public static DelegateCommand CreateCommand<T>(
            [NotNull] Action<T> execute,
            [CanBeNull] Predicate<T> canExecute = null)
        {
            return new DelegateCommand(
                o => execute((T) o),
                canExecute != null ? o => canExecute((T) o) : new Predicate<object>(o => true));
        }

        /// <summary>
        /// creates new instance of <see cref="DelegateCommand"/>.
        /// </summary>
        /// <param name="execute">delegate to execute command.</param>
        /// <param name="canExecute">delegate that takes an object to check whether command can be executed or not. can be null if command should be always executed.</param>
        [NotNull]
        public static DelegateCommand CreateCommand(
            [NotNull] Action execute,
            [CanBeNull] Predicate<object> canExecute = null)
        {
            return new DelegateCommand(o => execute(), canExecute ?? (o => true));
        }

        /// <summary>
        /// creates new instance of <see cref="DelegateCommand"/> that executes asynchronously.
        /// </summary>
        /// <typeparam name="T">type of parameter passed to execute and canExecute delegates.</typeparam>
        /// <param name="executeAsync">delegate that takes object of type <see cref="T"/> to execute command.</param>
        /// <param name="canExecute">delegate that takes object of type <see cref="T"/> to check whether command can be executed or not. can be null if command should be always executed.</param>
        [NotNull]
        public static DelegateCommand CreateAsyncCommand<T>(
            [NotNull] Func<T, Task> executeAsync,
            [CanBeNull] Predicate<T> canExecute = null)
        {
            return new DelegateCommand(
                o => executeAsync((T) o),
                canExecute != null ? o => canExecute((T) o) : new Predicate<object>(o => true));
        }


        /// <summary>
        /// creates new instance of <see cref="DelegateCommand"/> that executes asynchronously.
        /// </summary>
        /// <param name="executeAsync">delegate to execute command.</param>
        /// <param name="canExecute">delegate that takes an object to check whether command can be executed or not. can be null if command should be always executed.</param>
        [NotNull]
        public static DelegateCommand CreateAsyncCommand(
            [NotNull] Func<Task> executeAsync,
            [CanBeNull] Predicate<object> canExecute = null)
        {
            return new DelegateCommand(o => executeAsync(), canExecute ?? (o => true));
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        
        public void RaiseCommand(object parameter = null, object canExecuteParameter = null)
        {
            if (((ICommand) this).CanExecute(canExecuteParameter ?? parameter))
            {
                ((ICommand) this).Execute(parameter);
            }
        }

        public async Task RaiseCommandAsync(object parameter = null, object canExecuteParameter = null)
        {
            if (((ICommand) this).CanExecute(canExecuteParameter ?? parameter))
            {
                await ExecuteAsync(parameter);
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        void ICommand.Execute(object parameter)
        {
            _execute(parameter);
        }

        private async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                await _executeAsync(parameter);
            }
            catch (Exception) // todo catch exceptions!
            {
                throw;
            }
        }
    }
}
