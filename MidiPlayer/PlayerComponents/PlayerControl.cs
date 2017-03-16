using System;
using JetBrains.Annotations;
using Synthesizer.Device.Output.Managed;

namespace MidiPlayer.PlayerComponents
{
    using EventArgs = Extensions.EventArgs;
    using MidiStream;
    using PlaybackComponents;
    using Timers;

    /// <summary>
    /// provides basic control over <see cref="PlaybackControl"/>.
    /// </summary>
    public sealed class PlayerControl : IDisposable
    {
        private bool _isPlaying;
        private bool _isInitialized;
        private readonly MidiTimer _timer;
        private readonly PlaybackControl _control;

        internal event EventArgs PlaybackEnds;

        internal MIDIOUTCAPS GetOutputCapabilities => _control.GetOutputCapabilities;

        internal PlayerControl([NotNull] PlaybackData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            _timer = new MidiTimer();
            _control = new PlaybackControl(data);

            _control.PlaybackEnds += OnPlaybackEnds;

            data.TempoChanged += _timer.SetTempo; // bind tempo
            _timer.TempoChanged += data.SetTempo;

            _timer.Beat += _control.Move;
        }

        internal void Initialize([NotNull] MidiStream stream, bool initializeMidiDevice = true)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            if(_isInitialized)
                throw new InvalidOperationException("Player is already initialized and must be closed before re-initialization.");
            
            _isInitialized = true;
            _timer.Initialize(stream.Format.TimeDivision);
            _control.Initialize(stream.Tracks, initializeMidiDevice);
        }

        /// <summary>
        /// Stops playback if it's playing, otherwise starts.
        /// </summary>
        public void Toggle()
        {
            _isPlaying = !_isPlaying;
            if (_isPlaying) Start();
            else Pause();
        }

        /// <summary>
        /// Playback starts.
        /// </summary>
        public void Start()
        {
            _isPlaying = true;
            _timer?.Start();
            _control.Start();
        }

        /// <summary>
        /// Playback stops, and all sounds turn off.
        /// </summary>
        public void Pause()
        {
            _isPlaying = false;
            _timer?.Stop();
            _control.Pause();
        }

        /// <summary>
        /// Playback stops, all sounds turn off, And cue is reset.
        /// </summary>
        public void Stop()
        {
            _isPlaying = false;
            _timer?.Stop();
            _control?.Stop();
        }

        private void OnPlaybackEnds()
        {
            Stop();
            PlaybackEnds?.Invoke();
        }

        /// <summary>
        /// Playback stops, then seeks to given time, starts again if it was playing.
        /// </summary>
        /// <param name="time"></param>
        public void SeekTo(double time)
        {
            if (time < 0) throw new ArgumentOutOfRangeException(nameof(time));

            _timer?.Stop();
            _control?.Reset();
            _control?.Seek(time);
            if(_isPlaying) Start();
        }

        /// <summary>
        /// close playback.
        /// </summary>
        public void Close()
        {
            _timer?.Stop();
            _control?.Close();
            _isPlaying = false;
            _isInitialized = false;
        }

        public void Dispose()
        {
            _timer.Dispose();
            _control.Dispose();
        }
    }
}
