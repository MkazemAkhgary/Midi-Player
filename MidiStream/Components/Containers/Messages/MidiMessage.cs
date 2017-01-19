using System;
using JetBrains.Annotations;
using Utilities.Comparers;

namespace MidiStream.Components.Containers.Messages
{
    /// <summary>
    /// Base class for all midi messages. 
    /// </summary>
    public abstract class MidiMessage : IMidiMessage
    {
        public byte[] Data { get; }

        internal MidiMessage([NotNull]byte[] data)
        {
            if(data == null)
                throw new ArgumentNullException(nameof(data));

            Data = data;
        }

        public virtual bool Equals(IMidiMessage obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(Data, obj.Data)) return true;
            return ArrayComparer<byte>.Create().Equals(Data, obj.Data);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IMidiMessage);
        }

        public override int GetHashCode()
        {
            return ~ArrayComparer<byte>.Create().GetHashCode(Data);
        }
    }
}
