﻿using JetBrains.Annotations;
using Utilities.Helpers;

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
            Data = data;
        }

        #region Implemented Methods

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

        #endregion Implemented Methods
    }
}
