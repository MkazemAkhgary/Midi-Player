// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming

using JetBrains.Annotations;

namespace MidiStream.Components.Containers.Messages
{
    /// <summary>
    /// Initializes new instance of Midi <see cref="SysexMessage"/> Message.
    /// </summary>
    public sealed class SysexMessage : SysCommonMessage
    {
        internal SysexMessage([NotNull] byte[] data) : base(data)
        {
        }
    }
}
