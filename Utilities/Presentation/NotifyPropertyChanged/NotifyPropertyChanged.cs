using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Utilities.Presentation.NotifyPropertyChanged
{
    /// <summary>
    /// This class must be inherited by a View model.
    /// Notifies clients that any property of view model has been changed.
    /// Can be used to chain multiple View models. (eg. first view model notifies second view model for new changes. second view model notifies clients.)
    /// </summary>
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        private readonly NotifyPropertyChangedProvider _provider;
        private readonly PropertyResetter _resetter;

        public event PropertyChangedEventHandler PropertyChanged;

        protected NotifyPropertyChanged([CanBeNull] Type targetType, bool canResetToDefaults = true, bool enableAutoPropertyChangedNotification = true)
        {
            if (canResetToDefaults)
            {
                _resetter = new PropertyResetter(this, true);
            }
            if (enableAutoPropertyChangedNotification)
            {
                _provider = new NotifyPropertyChangedProvider(GetType(), targetType);
            }
        }

        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            RaisePropertyChanged(args);
        }
        
        private void OnPropertyChanged(string caller, string target)
        {
            RaisePropertyChanged(_provider.GetPropertyChangedEventArgs(caller, target));
        }

        private async void OnPropertyChangedAsync(int wait, string caller, string target)
        {
            RaisePropertyChanged(await _provider.GetPropertyChangedEventArgsAsync(wait, caller, target));
        }

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            if (args != null) PropertyChanged?.Invoke(this, args);
        }
        
        /// <summary>
        /// Sets the value of field. and notifies the target or caller if value changed.
        /// </summary>
        protected void SetValue<T>(ref T field, T value, [CallerMemberName, NotNull] string name = "", [CanBeNull]string target = null, bool forceNotify = false)
        {
            if (EqualityComparer<T>.Default.Equals(field, value) && !forceNotify) return;
            field = value;
            if (_provider != null) OnPropertyChanged(name, target);
        }

        /// <summary>
        /// Sets the value of field. reserves notify property changed for next time after wait period is elapsed. note that final value of property is notified to the target regardless of what value was passed to this method.
        /// </summary>
        protected void SetValueDelayed<T>(ref T field, T value, int wait, [CallerMemberName, NotNull] string name = "", [CanBeNull]string target = null)
        {
            if (wait < 0) throw new ArgumentOutOfRangeException(nameof(wait));
            
            field = value;
            if(_provider != null) OnPropertyChangedAsync(wait, name, target);
        }

        /// <summary>
        /// resetes properties of view model back to their default state. 
        /// </summary>
        public void Reset()
        {
            _resetter?.InvokeReset();
        }
    }
}
