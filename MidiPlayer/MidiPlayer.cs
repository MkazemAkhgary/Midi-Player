using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MidiStream;
using Utilities.Presentation.Commands;

namespace MidiPlayer
{
    using PlaybackComponents;
    using PlayerComponents;

    /// <summary>
    /// Controls playback of a sound from a .mid file.
    /// </summary>
    public sealed class MidiPlayer : IDisposable
    {
        private readonly PlayerControl _control;

        public event EventHandler PlaybackEnds;

        [NotNull]
        public PlayerVM Context { get; }

        public bool IsLoaded => Context.IsPlaybackLoaded;
        public bool IsPlaying => Context.IsPlaybackPlaying;
        public string GetMidiOutputDeviceInfo => _control.GetOutputCapabilities.ToString();

        public MidiPlayer()
        {
            var data = new PlaybackData();
            Context = new PlayerVM(data);
            _control = new PlayerControl(data);

            _control.PlaybackEnds += () => PlaybackEnds?.Invoke(this, EventArgs.Empty);

            Context.SeekTo = DelegateCommand.CreateCommand<double>(t => _control.SeekTo(t < 0 ? 0 : t));
        }

        /// <summary>
        /// Start playing.
        /// </summary>
        public void Start()
        {
            if (Context.IsPlaybackLoaded) _control.Start();
        }

        /// <summary>
        /// Pause.
        /// </summary>
        public void Pause()
        {
            if (Context.IsPlaybackLoaded) _control.Pause();
        }

        /// <summary>
        /// Stop playing and set cue to begining.
        /// </summary>
        public void Stop()
        {
            if (Context.IsPlaybackLoaded) _control.Stop();
        }

        /// <summary>
        /// Open file.
        /// </summary>
        /// <param name="path">file path.</param>
        /// <param name="initializeMidiDevice">specifies wether initialize midi device before playing file or not.</param>
        public async Task<bool> Open([NotNull] string path, bool initializeMidiDevice = true)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

            using (var reader = new MidiStreamReader(path))
            {
                var stream = await reader.GetStream();
                return OpenStream(stream, initializeMidiDevice);
            }
        }

        internal bool Open([NotNull] MidiStream.MidiStream stream, bool initializeMidiDevice = true)
        {
            return OpenStream(stream, initializeMidiDevice);
        }

        public async Task<bool> Open([NotNull] TrackInfo info, bool initializeMidiDevice = true)
        {
            if(info.Stream != null) return OpenStream(info.Stream, initializeMidiDevice);
            else return await Open(info.Path, initializeMidiDevice);
        }

        private bool OpenStream([NotNull] MidiStream.MidiStream stream, bool initializeMidiDevice = true)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            if (IsLoaded) Close();

            _control?.Initialize(stream, initializeMidiDevice);
            return true;
        }

        /// <summary>
        /// Close current player. in order to play aging must call <see cref="Open"/>
        /// </summary>
        public void Close()
        {
            if(IsPlaying) _control.Stop();
            _control?.Close();
        }

        public void Dispose()
        {
            _control.Dispose();
        }
    }
}
