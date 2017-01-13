using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Utilities.Threading;

namespace MidiPlayer.VMComponents
{
    internal sealed class NotifyPropertyChangedProvider
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly HybridDictionary _notifiers;

        public NotifyPropertyChangedProvider(NotifyPropertyChanged target)
        {
            _notifiers = new HybridDictionary();

            var props = target.GetType().GetProperties(Flags);

            foreach (var pInfo in props)
            {
                var args = new PropertyChangedEventArgs(pInfo.Name);
                var container = new BlockingContainer<PropertyChangedEventArgs>(args);
                _notifiers.Add(pInfo.Name, container);
            }
        }
        
        private BlockingContainer<PropertyChangedEventArgs> GetEventArgsContainer(string name, bool cache)
        {
            var container = (BlockingContainer<PropertyChangedEventArgs>)_notifiers[name];

            if (container == null && cache)
            {
                var notifier = new PropertyChangedEventArgs(name);
                container = new BlockingContainer<PropertyChangedEventArgs>(notifier);
                _notifiers.Add(name, container);
            }

            return container;
        }

        /// <summary>
        /// Notifies property changed.
        /// </summary>
        public PropertyChangedEventArgs GetPropertyChangedEventArgs([NotNull] string caller, [CanBeNull] string target = null, bool cache = false)
        {
            if (caller == null)
                throw new InvalidOperationException();

            if(target == null)
                return GetEventArgsContainer(caller, cache)?.RequestItem();
            else 
                return GetEventArgsContainer(target, true).RequestItem();
        }

        /// <summary>
        /// Notifies property changed after specified amount of time. incoming notifications are ignored until current notification is sent.
        /// </summary>
        public async Task<PropertyChangedEventArgs> GetPropertyChangedEventArgsAsync(int delay, [NotNull]string caller, [CanBeNull] string target = null, bool cache = false)
        {
            if (caller == null)
                throw new InvalidOperationException();

            if (target == null)
                return await (GetEventArgsContainer(caller, cache)?.Reserve(delay)
                              ?? Task.FromResult<PropertyChangedEventArgs>(null));
            else
                return await GetEventArgsContainer(target, true).Reserve(delay);
        }
    }
}
