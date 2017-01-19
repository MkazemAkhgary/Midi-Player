using System;
using System.Windows.Input;
using JetBrains.Annotations;

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
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            if (applicant == null) throw new ArgumentNullException(nameof(applicant));

            _execute = execute;
            _applicant = applicant;
        }

        [NotNull]
        public static DelegateCommand Create<T>(
            [NotNull] Action<T> execute, 
            [CanBeNull] Predicate<T> applicant = null)
        {
            return new DelegateCommand(obj => execute((T) obj), obj => applicant?.Invoke((T) obj) ?? true);
        }

        [NotNull]
        public static DelegateCommand Create(
            [NotNull] Action<object> execute,
            [CanBeNull] Predicate<object> applicant = null)
        {
            return new DelegateCommand(execute, applicant ?? (_ => true));
        }

        [NotNull]
        public static DelegateCommand Create(
            [NotNull] Action execute,
            [CanBeNull] Predicate<object> applicant = null)
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
