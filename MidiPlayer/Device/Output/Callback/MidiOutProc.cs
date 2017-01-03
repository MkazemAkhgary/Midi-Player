using System;

namespace MidiPlayer.Device.Output.Callback
{
    /// <summary>
    /// callback function for handling outgoing MIDI messages.
    /// </summary>
    /// <param name="midiOut">Handle to the MIDI device associated with the callback function.</param>
    /// <param name="message">MIDI output message.</param>
    /// <param name="instance">Instance data supplied by using the <see cref="Extensions.NativeMethods.MidiOutput.midiOutOpen"/> function.</param>
    /// <param name="param1">Message parameter.</param>
    /// <param name="param2">Message parameter.</param>
    internal delegate void MidiOutProc(IntPtr midiOut, uint message, IntPtr instance, IntPtr param1, IntPtr param2);
}
