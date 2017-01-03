namespace MidiPlayer.Timers
{
    using Extensions;

    internal interface IMidiTimer
    {
        event EventArgs<double, double> Beat;
        
        void SetTempo(double tempo);

        void Start();
        void Stop();
    }
}
