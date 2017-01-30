using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Utilities.Presentation.Commands;

namespace MidiPlayer.PlayerComponents
{
    using MidiStream;
    using PlaybackComponents;

    /// <summary>
    /// Controls playback of a sound from a .mid file.
    /// </summary>
    public sealed class MidiPlayer : IDisposable
    {
        private readonly PlayerControl _control;

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
            
            Context.SeekTo = DelegateCommand.CreateCommand<double>(_control.SeekTo);
        }

        public void Start()
        {
            if (Context.IsPlaybackLoaded) _control.Start();
        }

        public void Pause()
        {
            if (Context.IsPlaybackLoaded) _control.Pause();
        }

        public void Stop()
        {
            if (Context.IsPlaybackLoaded) _control.Stop();
        }

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

        private bool OpenStream([NotNull] MidiStream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            _control?.Initialize(stream);
            return true;
        }

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
