using System;
using System.Diagnostics.CodeAnalysis;
using MidiStream.Components.Header;

namespace Midi.PlaybackComponents
{
    using Extensions;
    using VMComponents;

    /// <summary>
    /// Model for keeping playback data.
    /// </summary>
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    internal sealed class PlaybackData : NotifyPropertyChanged
    {
        internal event EventArgs<double> TempoChanged;

        public PlaybackData() : base(useDefaultsOnReset: true)
        {
        }

        private bool _play = false;

        private double _stadur = 1;
        private double _rundur = 1;
        private double _stapos = 0;
        private double _runpos = 0;

        private double _pbspeed = 1;
        private double _tlen = TimeDivision.Default.GetResolution()/1000;
        private double _mspb = TimeDivision.Default.MicroSecondsPerBeat;

        public double StaticDuration
        {
            get { return _stadur; }
            set
            {
                if(_stadur == value) return;
                _stadur = value;
                OnPropertyChanged();
            }
        }

        public double StaticPosition
        {
            get { return _stapos; }
            set
            {
                if (_stapos == value) return;
                _stapos = value;
                OnPropertyChanged();
            }
        }

        public double RuntimeDuration
        {
            get { return _rundur; }
            set
            {
                if (_rundur == value) return;
                _rundur = value;
                OnPropertyChanged();
            }
        }

        public double RuntimePosition
        {
            get { return _runpos; }
            set
            {
                if (Math.Abs(_runpos - value) < 5) return; // every 500ms
                _runpos = value;
                OnPropertyChanged();
            }
        }

        public double TickLength
        {
            get { return _tlen; }
            set
            {
                if (_tlen == value) return;
                _tlen = value;
                OnPropertyChanged();
            }
        }

        public double PlaybackSpeed
        {
            get { return _pbspeed; }
            set
            {
                RuntimeDuration /= value / _pbspeed; // correction

                _pbspeed = value;
                TempoChanged?.Invoke(_mspb/_pbspeed);
            }
        }

        public double MicrosecondsPerBeat
        {
            get { return _mspb; }
            set
            {
                _mspb = value;
                TempoChanged?.Invoke(_mspb/_pbspeed);
            }
        }

        public bool IsPlaying
        {
            get { return _play; }
            set
            {
                if(value == _play) return;
                _play = value;
                OnPropertyChanged();
            }
        }

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
