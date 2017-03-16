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

            Context.SeekTo = DelegateCommand.CreateCommand<double>(_control.SeekTo);
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
        /// <returns></returns>
        public async Task<bool> Open([NotNull] string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

            using (var reader = new MidiStreamReader(path))
            {
                if(IsLoaded) Close();
                var stream = await reader.GetStream();
                return OpenStream(stream);
            }
        }

        private bool OpenStream([NotNull] MidiStream.MidiStream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            _control?.Initialize(stream);
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
