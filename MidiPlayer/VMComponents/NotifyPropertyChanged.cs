using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Utilities.Threading;

namespace MidiPlayer.VMComponents
{
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged, IDisposable
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public event PropertyChangedEventHandler PropertyChanged;

        private BlockingContainer<PropertyChangedEventArgs> GetEventArgsContainer(string name)
        {
            return (BlockingContainer<PropertyChangedEventArgs>)_buffer[name];
        }

        protected void SetValue<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            OnPropertyChanged(name);
        }

        protected void SetValueDelayed<T>(ref T field, T value, int delay, [CallerMemberName] string name = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            OnPropertyChangedDelayed(delay, name);
        }

        /// <summary>
        /// Notifies property changed.
        /// </summary>
        private void OnPropertyChanged(string caller)
        {
            RaisePropertyChanged(GetEventArgsContainer(caller).RequestItem());
        }

        /// <summary>
        /// Notifies property changed after specified amount of time. incoming notifications are ignored until current notification is sent.
        /// </summary>
        private void OnPropertyChangedDelayed(int delay, string caller)
        {
            GetEventArgsContainer(caller).Reserve(delay);
        }

        /// <summary>
        /// Notifies property changed.
        /// </summary>
        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            RaisePropertyChanged(args);
        }

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            if (args != null) PropertyChanged?.Invoke(this, args);
        }
        
        protected NotifyPropertyChanged(bool useDefaultsOnReset)
        {
            // prepare property event args from inheriting class for life time use.
            _buffer = new HybridDictionary();
            var props = GetType().GetProperties(Flags);

            if (useDefaultsOnReset)
            {
                var defaults = props.Select(GetAll);
                _resetters = props.Zip(defaults, SetAll).ToArray();
            }
            else
            {
                _resetters = props.Select(SetAll).ToArray();
            }

            foreach (var pInfo in props)
            {
                var args = new PropertyChangedEventArgs(pInfo.Name);
                var container = new BlockingContainer<PropertyChangedEventArgs>(args, RaisePropertyChanged);
                _buffer.Add(pInfo.Name, container);
            }
        }
        
        private object GetAll(PropertyInfo pInfo) => pInfo.GetValue(this);
        private Action SetAll(PropertyInfo pInfo, object obj) => () => pInfo.SetValue(this, obj);
        private Action SetAll(PropertyInfo pInfo, int _) => () => pInfo.SetValue(this, null);

        /// <summary>
        /// Resetes properties back to their default state.
        /// </summary>
        internal void Reset()
        {
            foreach (var resetter in _resetters)
            {
                resetter();
            }
        }

        private readonly Action[] _resetters;
        private readonly HybridDictionary _buffer;
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;

            foreach (BlockingContainer<PropertyChangedEventArgs> bc in _buffer.Values)
            {
                bc.Dispose();
            }
            _buffer.Clear();
        }
    }
}
