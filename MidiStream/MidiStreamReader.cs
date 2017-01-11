using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utilities.Helpers;
// ReSharper disable MemberCanBePrivate.Global

namespace MidiStream
{
    using Components.Containers.Events;
    using Components.Containers.Messages;
    using Components.Containers.Tracks;
    using Components.Header;
    using Exceptions;
    using Enums;
    using static Components.Containers.Messages.Factory.MessageFactory;
    using static IntConverter;

    /// <summary>
    /// Initializes a new instance of <see cref="MidiStreamReader"/> with specified path.
    /// </summary>
    public sealed class MidiStreamReader : IDisposable
    {
        private readonly BinaryReader _reader;
        private MidiStream _stream;

        public string FullPath { get; }

        #region Constructors

        public MidiStreamReader(string path, Encoding encoding)
        {
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 8192);
            _reader = new BinaryReader(stream, encoding);
        }

        public MidiStreamReader(string path) : this(path, Encoding.ASCII)
        {
            FullPath = path;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// gets the parsed midi stream.
        /// </summary>
        public MidiStream GetStream()
        {
            if (_stream != null) return _stream;

            var name = Path.GetFileNameWithoutExtension(FullPath);
             
            // read header
            var header = _reader.ReadChars(4);
            var length = Reverse(_reader.ReadInt32());
            var type = Reverse(_reader.ReadInt16());
            var numberOfTracks = Reverse(_reader.ReadInt16());
            var timedivision = Reverse(_reader.ReadInt16());

            // validate header
            if (!header.SequenceEqual("MThd") // check header
                || length != 6 // check length
                || !Enum.IsDefined(typeof(MidiType), type))
            {
                throw new MidiStreamException(MidiException.NotMidi);
            }

            // read all tracks
            var tracks = new MidiTrack[numberOfTracks];
            for (int i = 0; i < numberOfTracks; i++)
            {
                tracks[i] = ReadTrack();
            }

            var format = new MidiFormat((MidiType) type, timedivision);

            return _stream = new MidiStream(tracks, format, name);
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        #endregion Public Methods

        #region Private Methods

        private MidiTrack ReadTrack()
        {
            var header = _reader.ReadChars(4);
            if (!header.SequenceEqual("MTrk")) // check header 
            {
                throw new MidiStreamException(MidiException.TrackMissing);
            }

            return new MidiTrack(GetEvents());
        }

        /// <summary>
        /// Lazily reads all events within the track.
        /// </summary>
        /// <returns>events within the track.</returns>
        private IEnumerable<IMidiEvent<MidiMessage>> GetEvents()
        {
            long totalTicks = 0;
            var length = Reverse(_reader.ReadInt32());
            var pos = _reader.BaseStream.Position;

            while (_reader.BaseStream.Position - pos < length)
            {
                yield return ReadEvent(ref totalTicks);
            }
        }

        // holds active status in case next status was ommited.
        private byte _currentStatus;

        private IMidiEvent<MidiMessage> ReadEvent(ref long totalTicks)
        {
            totalTicks += ReadVariableLength();

            // parse message
            var status = _reader.ReadByte();
            if ((status & 0x80) == 0)
            {
                status = _currentStatus;
                _reader.BaseStream.Seek(-1, SeekOrigin.Current); // step back
            }
            else
                _currentStatus = status;
            
            byte[] data;


            if (status >= 0x80 && status <= 0xEF) // voice message. most common
            {
                data = new byte[] { status, _reader.ReadByte(), 0 };

                if (status < 0xC0 || status > 0xDF)
                {
                    data[2] = _reader.ReadByte();
                }

                return new MidiEvent<VoiceMessage>(totalTicks, CreateMessage<VoiceMessage>(data));
            }
            else if (status == 0xFF) // meta message. 
            {
                var type = _reader.ReadByte();
                var length = _reader.ReadByte();
                var content = _reader.ReadBytes(length);
                data = new[] { status, type }.Concat(content).ToArray();
                return new MidiEvent<MetaMessage>(totalTicks, CreateMessage<MetaMessage>(data));
            }
            else if(status >= 0xF0 && status <= 0xF6) // system common message
            {
                if (status == 0xF0) // system exclusive message
                {
                    var id = _reader.ReadByte();
                    var pos = _reader.BaseStream.Position;
                    while (_reader.ReadByte() != 0x7F) { }
                    var newpos = _reader.BaseStream.Position;
                    var length = newpos - pos;
                    _reader.BaseStream.Seek(-length, SeekOrigin.Current);
                    data = new[] {status, id}.Concat(_reader.ReadBytes((int) length)).ToArray();
                    return new MidiEvent<SysexMessage>(totalTicks, CreateMessage<SysexMessage>(data));
                }

                switch ((SystemCommon)status)
                {
                    case SystemCommon.MidiTimeCodeQuarterFrame:
                    case SystemCommon.SongSelect:
                        data = new[] {status, _reader.ReadByte()};
                        break;
                    case SystemCommon.SongPositionPointer:
                        data = new[] {status, _reader.ReadByte(), _reader.ReadByte()};
                        break;
                    case SystemCommon.TuneRequest:
                        data = new[] {status};
                        break;
                    default:
                        throw new MidiStreamException(MidiException.NotSupported);
                }

                return new MidiEvent<SysCommonMessage>(totalTicks, CreateMessage<SysCommonMessage>(data));
            }
            else if (status >= 0xF8 && status <= 0xFF) // system realtime
            {
                data = new[] {status};
                return new MidiEvent<SysRealtimeMessage>(totalTicks, CreateMessage<SysRealtimeMessage>(data));
            }

            throw new MidiStreamException(MidiException.NotSupported);
        }

        /// <summary>
        /// Reads the variable length quantity (VQL)
        /// </summary>
        /// <returns>VQL</returns>
        private int ReadVariableLength()
        {
            int value = 0;
            int b;
            do
            {
                b = _reader.ReadByte();
                value = (value << 7) | (b & 0x7F);
                if (value > 0x0FFFFFFF) // maximum length allowed is 4 bytes.
                {
                    // _reader.BaseStream.Seek(-1, SeekOrigin.Current);
                    throw new MidiStreamException(MidiException.VLQ_Overflow);
                }
            } while ((b & 0x80) != 0);
            return value;
        }

        #endregion
    }
}
