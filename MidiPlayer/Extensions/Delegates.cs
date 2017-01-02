namespace Midi.Extensions
{
    internal delegate void EventArgs();

    internal delegate void EventArgs<in T1>(T1 arg1);

    internal delegate void EventArgs<in T1, in T2>(T1 arg1, T2 arg2);
}