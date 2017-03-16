using System;
using System.IO;
using System.Threading.Tasks;
using Utilities.Presentation.NotifyPropertyChanged;

namespace MidiPlayer
{
    /// <summary>
    /// provides basic information for midi tracks.
    /// </summary>
    public class TrackInfo : NotifyPropertyChanged
    {
        private TimeSpan _duration;

        /// <summary>
        /// gets directory of track.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// gets name of track
        /// </summary>
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
            using (var temp = new MidiPlayer())
            {
                await temp.Open(Path, false);
                Duration = TimeSpan.FromMilliseconds(temp.Context.RuntimeDuration);
            }
        }
    }
}
