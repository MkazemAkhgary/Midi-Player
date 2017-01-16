using JetBrains.Annotations;

namespace MidiStream.Components.Containers.Messages
{
    /// <summary>
    /// Initializes new instance of Midi <see cref="SysexMessage"/> Message.
    /// </summary>
    [UsedImplicitly]
    public sealed class SysexMessage : SysCommonMessage
    {
        internal SysexMessage([NotNull] byte[] data) : base(data)
        {
        }
    }
}
