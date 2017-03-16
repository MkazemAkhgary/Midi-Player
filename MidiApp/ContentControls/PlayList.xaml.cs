using System.Windows.Controls;
using System.Windows.Input;
using MidiApp.ViewModel;

namespace MidiApp.ContentControls
{
    /// <summary>
    /// Interaction logic for PlayList.xaml
    /// </summary>
    public partial class PlayList
    {
        public PlayList()
        {
            InitializeComponent();
        }

        private async void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            await ((Player) DataContext).Select.RaiseCommandAsync(((ListViewItem) sender).Content);
        }
    }
}
