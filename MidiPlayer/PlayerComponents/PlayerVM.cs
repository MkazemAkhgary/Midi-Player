using System.Windows.Input;
// ReSharper disable InconsistentNaming
// ReSharper disable ArgumentsStyleLiteral

namespace MidiPlayer.PlayerComponents
{
    using PlaybackComponents;
    using VMComponents;
    
    public class PlayerVM : NotifyPropertyChanged
    {
        internal PlayerVM(PlaybackData data) : base(useDefaultsOnReset: false, enableAutoPropertyChangedNotification: false)
        {
            Data = data;
            Data.PropertyChanged += OnPropertyChanged;
        }

        private PlaybackData Data { get; }

        public bool IsPlaybackLoaded
        {
            get { return Data.IsLoaded; }
            set { Data.IsLoaded = value; }
        }

        public bool IsPlaybackPlaying
        {
            get { return Data.IsPlaying; }
            set { Data.IsPlaying = value; }
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
        
        public ICommand Toggle { get; internal set; }
        public ICommand SeekTo { get; internal set; }
    }
}
