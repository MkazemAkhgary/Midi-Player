using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MidiStream.Components.Containers.Events;
using MidiStream.Components.Containers.Messages;

namespace Midi.Sequencer
{
    using Containers = MidiStream.Components.Containers;

    /// <summary>
    /// Provides basic iteration over sequence of events, seeking and querying specific filtered events.
    /// </summary>
    internal partial class Sequence<TKey, TValue> : IDisposable, IEnumerable<MidiEvent<TValue>> where TValue : MidiMessage
    {
        private Enumerator _enumerator;

        public bool Ends => _enumerator.Ends;

        public Sequence(Containers::IGrouping<TKey, MidiEvent<TValue>>  source)
        {
            _enumerator = new Enumerator(source);
            _enumerator.MoveNext();
        }

        #region Delegatees

        /// <summary>
        /// get all events from specified keys.
        /// </summary>
        /// <param name="include">indicates whether include or exclude keys.</param>
        /// <param name="keys">which keys to include or exclude.</param>
        public IEnumerable<MidiEvent<TValue>> All(bool include, TKey[] keys)
        {
            return include
                ? _enumerator.Including(keys)
                : _enumerator.Excluding(keys);
        }

        /// <summary>
        /// get next events after current events up to given position and update current position.
        /// </summary>
        public IEnumerable<TValue> Next(double position)
        {
            while (position > _enumerator.Current?.AbsoluteTicks)
            {
                yield return _enumerator.Current.Message;
                if(!_enumerator.MoveNext()) break;
            }
        }

        /// <summary>
        /// Seeks enumerator to nearest event to target using binary search.
        /// </summary>
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public void Seek(double target)
        {
            _enumerator.Reset();

            if (target == 0 || !this.Any()) return;

            if (target > this.Last().AbsoluteTicks) // track ends
            {
                _enumerator.Seek(this.Count());
                return;
            }

            int limiter = this.Count();
            int expander = 0;

            while (expander <= limiter) // binary search
            {
                int midpoint = (expander + limiter) / 2;
                var element = this.ElementAt(midpoint);
                if (target > element.AbsoluteTicks)
                {
                    expander = midpoint + 1;
                }
                else if (target < element.AbsoluteTicks)
                {
                    limiter = midpoint - 1;
                }
                else if (target - element.AbsoluteTicks == 0)
                {
                    _enumerator.Seek(midpoint);
                    return;
                }
            }

            _enumerator.Seek(limiter);
        }

        #endregion Delegatees

        public void Dispose()
        {
            _enumerator.Dispose();
        }
    }
}
