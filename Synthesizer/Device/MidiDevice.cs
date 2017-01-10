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
        private readonly IntPtr _invalidHandle;

        protected MidiDevice() : base(IntPtr.Zero, true)
        {
            _invalidHandle = IntPtr.Zero;
        }

        #region Properties

        public override bool IsInvalid => handle == _invalidHandle;

        protected static MMRESULT LastError
        {
            set
            {
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