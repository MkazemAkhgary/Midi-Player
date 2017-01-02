using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Midi.Device.Output.Callback
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [Flags]
    internal enum CALLBACK : uint
    {
        [Description("The callback parameter is an event handle.This callback mechanism is for output only.")]
        CALLBACK_EVENT = 1,
        [Description("The callback parameter is a callback function address.")]
        CALLBACK_FUNCTION,
        [Description("There is no callback mechanism.This value is the default setting.")]
        CALLBACK_NULL,
        [Description("The callback parameter is a thread identifier.")]
        CALLBACK_THREAD,
        [Description("The callback parameter is a window handle.")]
        CALLBACK_WINDOW
    }
}
