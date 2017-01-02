using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace Midi.PlayerComponents
{
    using PlaybackComponents;
    using VMComponents;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PlayerVM : NotifyPropertyChanged
    {
        internal PlayerVM(PlaybackData data) : base(useDefaultsOnReset: false)
        {
            Data = data;
            Data.PropertyChanged += OnPropertyChanged;
        }

        private PlaybackData Data { get; }

        public bool IsPlaybackLoaded
        {
            get { return Data.IsPlaybackLoaded; }
            set { Data.IsPlaybackLoaded = value; }
        }

        public double StaticDuration
        {
            get { return Data.StaticDuration; }
            set { Data.StaticDuration = value; }
        }

        public double StaticPosition
        {
            get { return Data.StaticPosition; }
            set { Data.StaticPosition = value; }
        }

        public double RuntimeDuration
        {
            get { return Data.RuntimeDuration; }
            set { Data.RuntimeDuration = value; }
        }

        public double RuntimePosition
        {
            get { return Data.RuntimePosition; }
            set { Data.RuntimePosition = value; }
        }

        public double PlaybackSpeed
        {
            get { return Data.PlaybackSpeed; }
            set { Data.PlaybackSpeed = value; }
        }

        public bool IsPlaying
        {
            get { return Data.IsPlaying; }
            set { Data.IsPlaying = value; }
        }
        
        public ICommand Toggle { get; internal set; }
        public ICommand SeekTo { get; internal set; }
    }
}
