using System;
using MidiPlayer.PlayerComponents;
using MidiStream.Components.Header;
using Utilities.Presentation.NotifyPropertyChanged;

namespace MidiPlayer.PlaybackComponents
{
    using Extensions;

    /// <summary>
    /// Model for keeping playback status. this model is attached to <see cref="PlayerVM"/>. any changes to its properties will notify <see cref="PlayerVM"/>
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

        private double _stadur = 1; // total duration that is not affected by playback speed.
        private double _rundur = 1; // total duration that is affected by playback speed.
        private double _stapos = 0; // cue postion that is not affected by playback speed.
        private double _runpos = 0; // cue postion that is affected by playback speed.

        private double _pbspeed = 1; // playback speed
        private double _tlen = TimeDivision.Default.GetResolution() / 1000; // tick length
        private double _mspb = TimeDivision.Default.MicroSecondsPerBeat; // micro seconds per beat

        #endregion

        /// <summary>
        /// total duration that is not affected by playback speed.
        /// </summary>
        public double StaticDuration
        {
            get { return _stadur; }
            set { SetValue(ref _stadur, value); }
        }

        /// <summary>
        /// total duration that is affected by playback speed.
        /// </summary>
        public double RuntimeDuration
        {
            get { return _rundur; }
            set { SetValue(ref _rundur, value); }
        }

        /// <summary>
        /// cue postion that is not affected by playback speed.
        /// </summary>
        public double StaticPosition
        {
            get { return _stapos; }
            set { SetValueDelayed(ref _stapos, value, wait: 500); }
        }

        /// <summary>
        /// cue postion that is affected by playback speed.
        /// </summary>
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
                if(value <= 0) throw new ArgumentOutOfRangeException(nameof(value));

                TempoChanged?.Invoke(_mspb / value); // reset tempo

                // correction
                var ratio = RuntimePosition/RuntimeDuration;
                RuntimeDuration /= value / _pbspeed; 
                RuntimePosition = RuntimeDuration*ratio;

                SetValue(ref _pbspeed, value);
            }
        }

        public double MicrosecondsPerBeat
        {
            get { return _mspb; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));

                TempoChanged?.Invoke(value / _pbspeed);
                SetValue(ref _mspb, value);
            }
        }

        /// <summary>
        /// specifies whether player is playing or not.
        /// </summary>
        public bool IsPlaying
        {
            get { return _play; }
            set { SetValue(ref _play, value && _load, target: nameof(PlayerVM.IsPlaybackPlaying), forceNotify:true); }
        }

        /// <summary>
        /// specifies whether player has loaded midi stream or not.
        /// </summary>
        public bool IsLoaded
        {
            get { return _load; }
            set { SetValue(ref _load, value, target: nameof(PlayerVM.IsPlaybackLoaded), forceNotify:true); }
        }

        #endregion
        
        /// <summary>
        /// sets new tempo for current sequence.
        /// </summary>
        /// <param name="tempo">new tempo to set.</param>
        public void SetTempo(double tempo)
        {
            if (tempo < 0) throw new ArgumentOutOfRangeException(nameof(tempo));

            TickLength = tempo;
        }

        /// <summary>
        /// resets position to begining of the sequence.
        /// </summary>
        internal void ResetPosition()
        {
            StaticPosition = 0;
            RuntimePosition = 0;
            MicrosecondsPerBeat = TimeDivision.Default.MicroSecondsPerBeat;
            TickLength = TimeDivision.Default.GetResolution() / 1000;
            IsPlaying = false;
        }
    }
}
