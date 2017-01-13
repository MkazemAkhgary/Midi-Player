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

        protected NotifyPropertyChanged(bool useDefaultsOnReset = true, bool enableAutoPropertyChangedNotification = true)
        {
            _resetter = new PropertyResetter(this, useDefaultsOnReset);

            if (enableAutoPropertyChangedNotification)
            {
                _provider = new NotifyPropertyChangedProvider(this);
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

        private async void OnPropertyChangedAsync(int delay, string caller, string target)
        {
            RaisePropertyChanged(await _provider.GetPropertyChangedEventArgsAsync(delay, caller, target));
        }

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            if (args != null) PropertyChanged?.Invoke(this, args);
        }
        
        protected void SetValue<T>(ref T field, T value, [CallerMemberName, NotNull] string name = "", [CanBeNull]string target = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            if (_provider != null) OnPropertyChanged(name, target);
        }

        protected void SetValueDelayed<T>(ref T field, T value, int delay, [CallerMemberName, NotNull] string name = "", [CanBeNull]string target = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            if(_provider != null) OnPropertyChangedAsync(delay, name, target);
        }

        internal void Reset()
        {
            _resetter.InvokeReset();
        }
    }
}
