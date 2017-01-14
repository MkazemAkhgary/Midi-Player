using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace MidiPlayer.VMComponents
{
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        private readonly NotifyPropertyChangedProvider _provider;
        private readonly PropertyResetter _resetter;

        public event PropertyChangedEventHandler PropertyChanged;

        protected NotifyPropertyChanged(Type targetType, bool canResetToDefaults = true, bool enableAutoPropertyChangedNotification = true)
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
        protected void SetValue<T>(ref T field, T value, [CallerMemberName, NotNull] string name = "", [CanBeNull]string target = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            if (_provider != null) OnPropertyChanged(name, target);
        }

        /// <summary>
        /// Sets the value of field. and notifies the target or caller if value changed and aquires a wait for specified amount of time if wait is not aquired.
        /// </summary>
        protected void SetValueDelayed<T>(ref T field, T value, int wait, [CallerMemberName, NotNull] string name = "", [CanBeNull]string target = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            if(_provider != null) OnPropertyChangedAsync(wait, name, target);
        }

        internal void Reset()
        {
            _resetter?.InvokeReset();
        }
    }
}
