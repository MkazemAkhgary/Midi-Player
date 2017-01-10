using System.Collections;
using System.Collections.Generic;
using MidiStream.Components.Containers.Events;
using Utilities.Collections;

namespace MidiPlayer.Sequencer
{
    internal partial class Sequence<TKey, TValue>
    {
        #region Iterator
        /// <summary>
        /// This iterator keeps its state. it must be manualy reset when needed.
        /// </summary>
        private struct Enumerator : IEnumerator<MidiEvent<TValue>>
        {
            private readonly IGrouping<TKey, MidiEvent<TValue>> _source;
            private int _ind;

            public Enumerator(IGrouping<TKey, MidiEvent<TValue>> source)
            {
                _source = source;
                _ind = -1;
            }

            #region IEnumerator Imp
            public MidiEvent<TValue> Current => _ind < _source.Count ? _source[_ind] : null;
            object IEnumerator.Current => Current;
            public bool MoveNext() => ++_ind < _source.Count;
            public void Reset() => _ind = 0;
            public void Dispose() => _source.Clear();
            #endregion IEnumerator Imp

            public bool Ends => _ind >= _source.Count;
            public void Seek(int ind) =>  _ind = ind;
            public IEnumerable<MidiEvent<TValue>> Including(TKey[] keys) => _source.Including(keys);
            public IEnumerable<MidiEvent<TValue>> Excluding(TKey[] keys) => _source.Excluding(keys);
            internal IGrouping<TKey, MidiEvent<TValue>> GetSource() => _source;
        }
        #endregion Iterator

        public IEnumerator<MidiEvent<TValue>> GetEnumerator() => _enumerator.GetSource().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
