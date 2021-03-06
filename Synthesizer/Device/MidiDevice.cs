﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Synthesizer.Device
{
    using Extensions;

    /// <summary>
    /// Provides a safe handle for midi devices.
    /// </summary>
    public abstract class MidiDevice : SafeHandle
    {
        private static readonly IntPtr InvalidHandle = IntPtr.Zero;
        private static MMRESULT _error = MMRESULT.MMSYSERR_NOERROR;

        protected MidiDevice() : base(InvalidHandle, true)
        {
        }

        #region Properties

        public override bool IsInvalid => handle == InvalidHandle || IsClosed;

        protected static MMRESULT LastError
        {
            get { return _error; }
            set
            {
                _error = value;
                if (value == MMRESULT.MMSYSERR_NOERROR) return;
                const int maxlength = 256;
                var builder = new StringBuilder(maxlength);
                LastError = (MMRESULT)NativeMethods.MidiOutput.midiOutGetErrorText(value, builder, maxlength);
                throw new ApplicationException(builder.ToString());
            }
        }

        #endregion Properties

        protected abstract override bool ReleaseHandle();
    }
}
