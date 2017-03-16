using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MidiStream.Components.Containers.Tracks;
using MidiStream.Enums;
using Synthesizer.Device.Output;
using Synthesizer.Device.Output.Managed;
using Utilities.Extensions;

namespace MidiPlayer.PlaybackComponents
{
    using EventArgs = Extensions.EventArgs;
    using Dispatchers;

    /// <summary>
    /// provides basic control over <see cref="Playback"/>
    /// </summary>
    internal partial class PlaybackControl : IDisposable
    {
        private static readonly IReadOnlyCollection<Playback> NoPlayback;

        internal event EventArgs PlaybackEnds;

        private IReadOnlyCollection<Playback> _tracks;
        private readonly PlaybackData _data;
        private readonly Lazy<MidiOutput> _outputInit;

        [NotNull]
        private MidiOutput Output => _outputInit.Value;

        internal MIDIOUTCAPS GetOutputCapabilities => Output.Capabilities;

        static PlaybackControl()
        {
            NoPlayback = Enumerable.Empty<Playback>().ToList().AsReadOnly();
        }

        public PlaybackControl([NotNull] PlaybackData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            _tracks = NoPlayback;
            _data = data;
            _outputInit = new Lazy<MidiOutput>(() => new MidiOutput());
        }

        public void Initialize([NotNull] IReadOnlyList<MidiTrack> tracks, bool initializeMidiDevice = true)
        {
            if (tracks == null) throw new ArgumentNullException(nameof(tracks));
            if(_data.IsLoaded)
                throw new InvalidOperationException("Playback is already loaded and must be closed before re-initialization.");

            _data.IsLoaded = true;
            _tracks = tracks.ToReadOnlyCollection(t => new Playback(t));
            _data.StaticDuration = tracks.Max(t => t.TotalTicks);
            _data.RuntimeDuration = CalculateRuntimePosition(_data.StaticDuration);

            if(initializeMidiDevice) Reset();
        }

        public void Seek(double position)
        {
            if (position < 0) throw new ArgumentOutOfRangeException(nameof(position));

            _data.StaticPosition = CalculateStaticPosition(position);
            _data.RuntimePosition = position;

            foreach (var track in _tracks)
            {
                track.Seek(_data.StaticPosition);
            }

            _tracks.SelectMany(
                    seq => seq.GetVoiceEvents(include: false, keys: new[] {VoiceType.NoteOn, VoiceType.NoteOff}))
                .TakeWhile(e => e.AbsoluteTicks <= _data.StaticPosition)
                .OrderBy(e => e.AbsoluteTicks)
                .Select(e => e.Message).DispatchTo(Output);
        }

        public void Reset()
        {
            Output.Reset();
            _data.ResetPosition();

            foreach (var track in _tracks)
            {
                track.Seek(0);
            }
        }

        /// <summary>
        /// Move sequence forward. this handler firest at every midi timer interval.
        /// </summary>
        public void Move(double sta, double dyn)
        {
            _data.RuntimePosition += dyn;
            _data.StaticPosition += sta;

            foreach (var track in _tracks)
            {
                track.NextVoiceMessages(_data.StaticPosition).DispatchTo(Output);
                track.NextMetaMessages(_data.StaticPosition).DispatchTo(this);
            }

            if (_tracks.All(t => t.Ends))
                PlaybackEnds?.Invoke();
        }
        
        public void SetPlaybackSpeed(double mspb)
        {
            if (mspb <= 0) throw new ArgumentOutOfRangeException(nameof(mspb));

            _data.MicrosecondsPerBeat = mspb;
        }

        public void Pause()
        {
            _data.IsPlaying = false;
            Output.Mute();
        }

        public void Stop()
        {
            _data.IsPlaying = false;
            Reset();
        }

        public void Start()
        {
            _data.IsPlaying = true;
        }

        public void Close()
        {
            _tracks = NoPlayback;
            Output.Reset();
            _data.Reset();
        }

        public void Dispose()
        {
            if(_outputInit.IsValueCreated) Output.Dispose();
        }
    }
}
