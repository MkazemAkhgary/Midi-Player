using System;
using System.Windows.Input;
// ReSharper disable UnusedMember.Global

namespace MidiPlayer.Commands
{
    /// <summary>
    /// An <see cref="ICommand"/> whose delegates can be attached for Execute and CanExecute.
    /// </summary>
    internal class DelegateCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _applicant;

        private DelegateCommand(Action<object> execute, Predicate<object> applicant)
        {
            _execute = execute;
            _applicant = applicant;
        }

        public static DelegateCommand Create<T>(Action<T> execute, Predicate<T> applicant = null)
        {
            return new DelegateCommand(obj => execute((T) obj), obj => applicant?.Invoke((T) obj) ?? true);
        }

        public static DelegateCommand Create(Action<object> execute, Predicate<object> applicant = null)
        {
            return new DelegateCommand(execute, applicant ?? (_ => true));
        }

        public static DelegateCommand Create(Action execute, Predicate<object> applicant = null)
        {
            return new DelegateCommand(_ => execute(), applicant ?? (_ => true));
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return _applicant(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
