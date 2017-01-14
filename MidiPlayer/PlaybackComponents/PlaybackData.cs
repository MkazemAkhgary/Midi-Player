using MidiPlayer.PlayerComponents;
using MidiStream.Components.Header;
// ReSharper disable All

namespace MidiPlayer.PlaybackComponents
{
    using Extensions;
    using VMComponents;

    /// <summary>
    /// Model for keeping playback data.
    /// </summary>
    internal sealed class PlaybackData : NotifyPropertyChanged
    {
        internal event EventArgs<double> TempoChanged;

        public PlaybackData() : base(
            typeof(PlayerVM),
            canResetToDefaults: true,
            enableAutoPropertyChangedNotification: true)
        {
        }

        #region Fields

        private bool _play = false;
        private bool _load = false;

        private double _stadur = 1;
        private double _rundur = 1;
        private double _stapos = 0;
        private double _runpos = 0;

        private double _pbspeed = 1;
        private double _tlen = TimeDivision.Default.GetResolution() / 1000;
        private double _mspb = TimeDivision.Default.MicroSecondsPerBeat;

        #endregion
        
        public double StaticDuration
        {
            get { return _stadur; }
            set { SetValue(ref _stadur, value); }
        }

        public double RuntimeDuration
        {
            get { return _rundur; }
            set { SetValue(ref _rundur, value); }
        }
        
        public double StaticPosition
        {
            get { return _stapos; }
            set { SetValueDelayed(ref _stapos, value, wait: 500); }
        }

        public double RuntimePosition
        {
            get { return _runpos; }
            set { SetValueDelayed(ref _runpos, value, wait: 500); }
        }

        #region Streaming

        public double TickLength
        {
            get { return _tlen; }
            set { SetValue(ref _tlen, value); }
        }

        public double PlaybackSpeed
        {
            get { return _pbspeed; }
            set
            {
                RuntimeDuration /= value / _pbspeed; // correction
                TempoChanged?.Invoke(_mspb / value); // reset tempo
                SetValue(ref _pbspeed, value);
            }
        }

        public double MicrosecondsPerBeat
        {
            get { return _mspb; }
            set
            {
                TempoChanged?.Invoke(value / _pbspeed);
                SetValue(ref _mspb, value);
            }
        }

        public bool IsPlaying
        {
            get { return _play; }
            set { SetValue(ref _play, value, target: nameof(PlayerVM.IsPlaybackPlaying)); }
        }

        public bool IsLoaded
        {
            get { return _load; }
            set { SetValue(ref _load, value, target: nameof(PlayerVM.IsPlaybackLoaded)); }
        }

        #endregion
        
        public void SetTempo(double tempo)
        {
            TickLength = tempo;
        }

        public void ResetPosition()
        {
            StaticPosition = 0;
            RuntimePosition = 0;
            MicrosecondsPerBeat = TimeDivision.Default.MicroSecondsPerBeat;
            TickLength = TimeDivision.Default.GetResolution() / 1000;
            IsPlaying = false;
        }
    }
}
