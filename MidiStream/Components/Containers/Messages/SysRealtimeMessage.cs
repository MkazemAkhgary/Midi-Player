using System;
using JetBrains.Annotations;

namespace MidiStream.Components.Containers.Messages
{
    /// <summary>
    /// Initializes new instance of Midi <see cref="SysRealtimeMessage"/> Message.
    /// </summary>
    [UsedImplicitly]
    public sealed class SysRealtimeMessage : MidiMessage
    {
        internal SysRealtimeMessage([NotNull] byte[] data) : base(data)
        {
        }
    }
}
