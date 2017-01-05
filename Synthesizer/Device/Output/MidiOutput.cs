using System;
using System.Runtime.InteropServices;
using MidiStream.Components.Containers.Messages;
using MidiStream.Enums;

namespace Synthesizer.Device.Output
{
    using Managed;
    using Extensions;

    /// <summary>
    /// Midi out-put device.
    /// </summary>
    public sealed class MidiOutput : MidiDevice
    {
        private static readonly object Lock = new object();

        #region Properties
        
        internal static uint NumberOfDevices { get; }

        internal uint Id { get; }

        public MIDIOUTCAPS OutputCapabilities { get; }

        #endregion Properties

        #region Constructors

        static MidiOutput()
        {
            NumberOfDevices = NativeMethods.MidiOutput.midiOutGetNumDevs();
        }

        public MidiOutput()
        {
            lock (Lock)
            {
                LastError = NativeMethods.MidiOutput.midiOutOpen(out handle, NumberOfDevices - 1, null, IntPtr.Zero, 0);

                // get id
                IntPtr pointer;
                LastError = NativeMethods.MidiOutput.midiOutGetID(handle, out pointer);
                Id = (uint)pointer.ToInt32();

                // get device capabilities
                var size = Marshal.SizeOf(typeof(MIDIOUTCAPS));
                var moc = default(MIDIOUTCAPS);
                LastError = NativeMethods.MidiOutput.midiOutGetDevCaps(handle, ref moc, (uint)size);
                OutputCapabilities = moc;
            }
        }

        #endregion Constructors

        #region Public Methods

        public void SendMessage(uint msg)
        {
            lock (Lock)
            {
                LastError = NativeMethods.MidiOutput.midiOutShortMsg(handle, msg);
            }
        }

        public void Reset()
        {
            lock (Lock)
            {
                LastError = NativeMethods.MidiOutput.midiOutReset(handle);
            }
        }

        public void Mute()
        {
            ResetChannels(VoiceType.Controller, (byte)Controller.AllNotesOff, 0);
            ResetChannels(VoiceType.Controller, (byte)Controller.HoldPedal1, 0);
        }

        private void ResetChannels(VoiceType type, byte subtype, byte value)
        {
            for (int i = 0; i < 16; i++)
            {
                var status = (byte)((int)type | i);
                byte[] data = { status, subtype, value };
                SendMessage(VoiceMessage.PackMessage(data));
            }
        }

        #endregion Public Methods

        #region Impl

        protected override bool ReleaseHandle()
        {
            lock (Lock)
            {
                try
                {
                    LastError = NativeMethods.MidiOutput.midiOutClose(handle);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        #endregion Impl
    }
}
