using System;
using System.Linq;
using System.Reflection;

namespace MidiPlayer.VMComponents
{
    internal sealed class PropertyResetter
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly object _target;

        public PropertyResetter(object target, bool useDefaultsOnReset = false)
        {
            // properties must be both readable and writable.
            var props = GetType().GetProperties(Flags).Where(p => p.CanRead && p.CanWrite).ToArray();
            _target = target;

            if (useDefaultsOnReset)
            {
                var defaults = props.Select(GetAll);
                _resetters = props.Zip(defaults, SetAll).ToArray();
            }
            else
            {
                _resetters = props.Select(SetAll).ToArray();
            }
        }

        /// <summary>
        /// Resetes properties of the target back to their default state.
        /// </summary>
        public void InvokeReset()
        {
            foreach (var resetter in _resetters)
            {
                resetter();
            }
        }

        private readonly Action[] _resetters;

        private object GetAll(PropertyInfo pInfo) => pInfo.GetValue(_target);
        private Action SetAll(PropertyInfo pInfo, object obj) => () => pInfo.SetValue(_target, obj);
        private Action SetAll(PropertyInfo pInfo, int _) => () => pInfo.SetValue(_target, null);
    }
}
