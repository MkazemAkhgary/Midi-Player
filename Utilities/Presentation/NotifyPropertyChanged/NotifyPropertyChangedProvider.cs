using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Utilities.Threading;

namespace Utilities.Presentation.NotifyPropertyChanged
{
    /// <summary>
    /// Provides list of properties event arguments for <see cref="NotifyPropertyChanged"/>.
    /// </summary>
    internal sealed class NotifyPropertyChangedProvider
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly HybridDictionary _sourceNotifiers;
        private readonly HybridDictionary _targetNotifiers;

        private readonly Type _caller;
        private readonly Type _callee;

        public NotifyPropertyChangedProvider([NotNull]Type caller, [CanBeNull]Type callee)
        {
            if(caller == null) 
                throw new ArgumentNullException(nameof(caller));

            _caller = caller;
            _callee = callee;
            _sourceNotifiers = GetNotifiers(caller);
            _targetNotifiers = GetNotifiers(callee);
        }

        private static HybridDictionary GetNotifiers(IReflect type)
        {
            if (type == null) return null;

            var notifiers = new HybridDictionary();
            var props = type.GetProperties(Flags);
            foreach (var pInfo in props)
            {
                var args = new PropertyChangedEventArgs(pInfo.Name);
                var container = new BlockingContainer<PropertyChangedEventArgs>(args);
                notifiers.Add(pInfo.Name, container);
            }
            return notifiers;
        }
        
        private BlockingContainer<PropertyChangedEventArgs> GetEventArgsContainer(string caller, string target)
        {
            var container = _sourceNotifiers[caller] as BlockingContainer<PropertyChangedEventArgs>;
            if(container == null) throw new InvalidOperationException($"Invalid caller name for provided type, could not find property in {_caller}.");
            if (target == null) return container;
            if (_callee == null) throw new InvalidOperationException("Can not provide PropertyChangedEventArgs for null target.");
            container = _targetNotifiers[target] as BlockingContainer<PropertyChangedEventArgs>;
            if(container == null) throw new InvalidOperationException($"Invalid target name for provided type, could not find property in {_callee}.");
            return container;
        }

        /// <summary>
        /// retrives property event argument synchronously.
        /// </summary>
        /// <param name="caller">name of property changed.</param>
        /// <param name="target">name of target property of next view model. can be null if client should be notified of value change instead.</param>
        /// <returns></returns>
        public PropertyChangedEventArgs GetPropertyChangedEventArgs([NotNull] string caller, [CanBeNull] string target = null)
        {
            if (caller == null)
                throw new ArgumentNullException(nameof(caller));

            return GetEventArgsContainer(caller, target).RequestItem();
        }

        /// <summary>
        /// retrives property event argument asynchronously. value will be returned after specified amount of time.
        /// returns null if property event argument is already requested.
        /// </summary>
        /// <param name="wait">wait time in milliseconds before returning back to caller.</param>
        /// <param name="caller">name of property changed.</param>
        /// <param name="target">name of target property of next view model. can be null if client should be notified of value change instead.</param>
        public async Task<PropertyChangedEventArgs> GetPropertyChangedEventArgsAsync(int wait, [NotNull]string caller, [CanBeNull] string target = null)
        {
            if (wait < 0) throw new ArgumentOutOfRangeException(nameof(wait));
            if (caller == null)
                throw new ArgumentNullException(nameof(caller));

            return await GetEventArgsContainer(caller, target).Reserve(wait);
        }
    }
}
