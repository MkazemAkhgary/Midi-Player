using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.Win32;

namespace MidiUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        internal static void OpenFile()
        {
            (Current.FindResource("FileDialog") as OpenFileDialog)?.ShowDialog(Current.MainWindow);
        }

        internal static void LoadStream(string filename)
        {
            var ctrl = (Current.MainWindow as MainWindow)?.PlayerControl;

            if(ctrl == null) return;
            
            ctrl.Player.Close();

            try
            {
                ctrl.Player.Open(filename);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
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
