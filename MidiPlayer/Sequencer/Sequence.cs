using System;
using System.Collections.Generic;
using MidiStream.Components.Containers.Events;
using MidiStream.Components.Containers.Messages;
using UtilCollection = Utilities.Collections;

namespace MidiPlayer.Sequencer
{
    /// <summary>
    /// Provides basic iteration over sequence of events, seeking and querying specific filtered events.
    /// </summary>
    internal partial class Sequence<TKey, TValue> : IDisposable, IEnumerable<MidiEvent<TValue>> where TValue : MidiMessage
    {
        private Enumerator _enumerator;

        public bool Ends => _enumerator.Ends;

        public Sequence(UtilCollection.IGrouping<TKey, MidiEvent<TValue>>  source)
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
        public void Seek(double target)
        {
            _enumerator.Reset();

            var dummy = MidiEvent<TValue>.CreateEmpty((long) target);
            var index = _enumerator.GetSource().BinarySearch(dummy);

            if(index < 0) index = ~index - 1;
            if(index < 0) return;

            _enumerator.Seek(index);
        }

        #endregion Delegatees

        public void Dispose()
        {
            _enumerator.Dispose();
        }
    }
}
