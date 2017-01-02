using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MidiStream.Properties;

namespace Midi.VMComponents
{
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies Target
        /// </summary>
        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, (PropertyChangedEventArgs) _buffer[name]);
        }

        /// <summary>
        /// Notifies Framework
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

            foreach (var prop in props)
            {
                _buffer.Add(prop.Name, new PropertyChangedEventArgs(prop.Name));
            }
        }
        
        private object GetAll(PropertyInfo info) => info.GetValue(this);
        private Action SetAll(PropertyInfo info, object obj) => () => info.SetValue(this, obj);
        private Action SetAll(PropertyInfo info, int _) => () => info.SetValue(this, null);

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
    }
}
