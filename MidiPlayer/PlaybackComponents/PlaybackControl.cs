using System;
using System.Collections.Generic;
using System.Linq;
using MidiStream.Components.Containers.Tracks;
using MidiStream.Enums;
using Utilities.Helpers;
using Synthesizer.Device.Output;
using Synthesizer.Device.Output.Managed;

namespace MidiPlayer.PlaybackComponents
{
    using EventArgs = Extensions.EventArgs;
    using Dispatchers;

    internal partial class PlaybackControl : IDisposable
    {
        private static readonly IReadOnlyCollection<Playback> NoPlayback;

        public event EventArgs OnPlaybackEnds;

        private IReadOnlyCollection<Playback> _tracks;
        private readonly PlaybackData _data;
        private readonly Lazy<MidiOutput> _outputInit;

        private MidiOutput Output => _outputInit.Value;

        internal MIDIOUTCAPS GetOutputCapabilities => Output.Capabilities;

        static PlaybackControl()
        {
            NoPlayback = Enumerable.Empty<Playback>().ToList().AsReadOnly();
        }

        public PlaybackControl(PlaybackData data)
        {
            _tracks = NoPlayback;
            _data = data;
            _outputInit = new Lazy<MidiOutput>(() => new MidiOutput());
        }

        public void Initialize(IReadOnlyList<MidiTrack> tracks)
        {
            _tracks = tracks.ToReadOnlyCollection(t => new Playback(t));
            _data.StaticDuration = tracks.Max(t => t.TotalTicks);
            _data.RuntimeDuration = CalculateRuntimePosition(_data.StaticDuration);
            Reset();
        }

        public void Seek(double position)
        {
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
                OnPlaybackEnds?.Invoke();
        }
        
        public void SetPlaybackSpeed(double mspb)
        {
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
            _data.Dispose();
        }
    }
}
