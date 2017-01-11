using JetBrains.Annotations;
// ReSharper disable ClassNeverInstantiated.Global

namespace MidiStream.Components.Containers.Messages
{
    /// <summary>
    /// Initializes new instance of Midi <see cref="SysRealtimeMessage"/> Message.
    /// </summary>
    public sealed class SysRealtimeMessage : MidiMessage
    {
        internal SysRealtimeMessage([NotNull] byte[] data) : base(data)
        {
        }
    }
}
