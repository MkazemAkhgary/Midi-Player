using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Utilities.Properties;
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


        /// <summary>
        /// Notifies property changed, and frees notifying target.
        /// </summary>
        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChangedForceReset(string name)
        {
            var container = GetEventArgsContainer(name);
            PropertyChanged?.Invoke(this, container.FreeItem());
        }

        /// <summary>
        /// Notifies property changed.
        /// </summary>
        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            var container = GetEventArgsContainer(name);
            PropertyChanged?.Invoke(this, container.ForceGetItem());
        }

        /// <summary>
        /// Notifies property changed, and blocks notifying target for specified amount of time.
        /// </summary>
        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged(int waitPeriod, [CallerMemberName] string name = "")
        {
            var container = GetEventArgsContainer(name);

            if (container.IsFree)
            {
                PropertyChanged?.Invoke(this, container.GetItem(waitPeriod));
            }
        }

        /// <summary>
        /// Notifies property changed.
        /// </summary>
        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
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
                var eventArgs = new PropertyChangedEventArgs(pInfo.Name);
                var container = new BlockingContainer<PropertyChangedEventArgs>(eventArgs);
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
            foreach (BlockingContainer<PropertyChangedEventArgs> bc in _buffer.Values)
            {
                bc.Dispose();
            }

            _buffer.Clear();
        }
    }
}
