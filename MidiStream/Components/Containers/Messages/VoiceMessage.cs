using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace MidiStream.Components.Containers.Messages
{
    using Enums;

    /// <summary>
    /// Initializes new instance of Midi <see cref="VoiceMessage"/> Message.
    /// </summary>
    [UsedImplicitly]
    public sealed class VoiceMessage : MidiMessage, IChannelMessage
    {
        /// <summary>
        /// Gets the sub type of voice message.
        /// </summary>
        public VoiceType VoiceType => (VoiceType)(Data[0] & 0xF0);
        public int Channel => Data[0] & 0x0F;

        internal VoiceMessage([NotNull]byte[] data) : base(data)
        { 
            // data should be always 4 bytes. this is done for simplicity. 
            // actual data must be 2 or 3 bytes. 
            if (data.Length > 4 || data.Length < 2)
                throw new ArgumentException(@"invalid data length.", nameof(data));

            if(data.Length != 4)
            {
                var newdata = new byte[4];
                Array.Copy(data, newdata, data.Length);
                data = newdata;
            }

            if (!Enum.IsDefined(typeof(VoiceType), VoiceType))
                throw new InvalidEnumArgumentException(nameof(VoiceType), (int) VoiceType, typeof(VoiceType));

            if (data[1] > 0x7F || data[2] > 0x7F) // these must be always less than 128
                throw new ArgumentException(@"corrupt data", nameof(data));

            if (VoiceType == VoiceType.ProgramChange || VoiceType == VoiceType.ChannelPressure)
            {
                if (data[2] != 0)
                    throw new ArgumentException(@"invalid data length.", nameof(data));
            }

            if (data[3] != 0) // must be always 0
                throw new ArgumentException(@"invalid data length.", nameof(data));
        }

        /// <summary>
        /// packs voice message into single integer value.
        /// </summary>
        /// <returns></returns>
        public uint PackMessage() => PackMessage(Data);

        public static uint PackMessage([NotNull] byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            return BitConverter.ToUInt32(data, 0);
        }
    }
}