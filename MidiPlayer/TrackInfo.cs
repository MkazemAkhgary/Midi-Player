using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MidiStream;
using Utilities.Presentation.NotifyPropertyChanged;

namespace MidiPlayer
{
    /// <summary>
    /// provides basic information for midi tracks.
    /// </summary>
    public class TrackInfo : NotifyPropertyChanged
    {
        private TimeSpan _duration;
        private TrackStatus _status = TrackStatus.Loading;

        private const int MaxCacheSize = 1024; // maximum size in KB

        /// <summary>
        /// gets directory of track.
        /// </summary>
        [NotNull]
        public string Path { get; }

        [CanBeNull]
        public MidiStream.MidiStream Stream { get; private set; }

        /// <summary>
        /// gets name of track
        /// </summary>
        [NotNull]
        public string Name { get; }
        
        /// <summary>
        /// gets size of track in KB.
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// gets duration of track.
        /// </summary>
        public TimeSpan Duration
        {
            get { return _duration; }
            set
            {
                SetValue(ref _duration, value);
            }
        }

        public TrackStatus Status
        {
            get { return _status; }
            set
            {
                SetValue(ref _status, value);
            }
        }

        public TrackInfo(string fileName) : base(null, false, true)
        {
            var fileInfo = new FileInfo(fileName);

            if(!fileInfo.Exists)
                throw new FileNotFoundException();

            Path = fileName;
            Name = System.IO.Path.GetFileNameWithoutExtension(fileName);
            Size = (int) (fileInfo.Length/1024);
        }

        public async Task Initialize()
        {
            using (var reader = new MidiStreamReader(Path))
            {
                var stream = await reader.GetStream();
                using (var player = new MidiPlayer())
                {
                    player.Open(stream, false);
                    Duration = TimeSpan.FromMilliseconds(player.Context.RuntimeDuration);
                }

                if (Size <= MaxCacheSize) // if less than 1mb
                    Stream = stream;
            }
        }

        public enum TrackStatus
        {
            Loading, Ready, Playing, Error
        }
    }
}
