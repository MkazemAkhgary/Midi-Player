using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.Win32;
using MidiPlayer.PlayerComponents;

namespace MidiApp
{
    using ContentControls;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly Lazy<PlayerControl> Ctrl;
        private static Player Player => Ctrl.Value.Player;
        private new static MainWindow MainWindow => Current.MainWindow as MainWindow;
        private static OpenFileDialog OpenFileDialog => Current.FindResource("FileDialog") as OpenFileDialog;

        static App()
        {
            Ctrl = new Lazy<PlayerControl>(() => MainWindow?.PlayerControl);
        }

        internal static void OpenFile()
        {
            OpenFileDialog?.ShowDialog(Current.MainWindow);
        }

        internal static void LoadStream(string filename)
        {
            try
            {
                Player.Open(filename);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
$@"{exception.Message}
{exception.InnerException?.Message}", "Error!");

                Player.Close();
            }
        }

        private void FileDialog_OnFileOk(object sender, CancelEventArgs e)
        {
            var opd = sender as OpenFileDialog;
            if(opd == null) return;
            LoadStream(opd.FileName);
        }
    }
}
