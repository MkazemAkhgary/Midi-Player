using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Utilities.Extensions;
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
    using static NumericExtensions;

    /// <summary>
    /// Initializes a new instance of <see cref="MidiStreamReader"/> with specified path.
    /// </summary>
    public sealed partial class MidiStreamReader
    {
        /// <summary>
        /// gets the parsed midi stream.
        /// </summary>
        [NotNull]
        public MidiStream GetStreamSync()
        {
            if (_stream != null) return _stream;

            var name = Path.GetFileNameWithoutExtension(FullPath);

            // read header
            var header = _reader.ReadCharsSync(4);
            var length = Reverse(_reader.ReadInt32Sync());
            var type = Reverse(_reader.ReadInt16Sync());
            var numberOfTracks = Reverse(_reader.ReadInt16Sync());
            var timedivision = Reverse(_reader.ReadInt16Sync());

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

            var format = new MidiFormat((MidiType)type, timedivision);

            return _stream = new MidiStream(tracks, format, name);
        }

        private MidiTrack ReadTrack()
        {
            var header = _reader.ReadCharsSync(4);
            if (!header.SequenceEqual("MTrk")) // check header 
            {
                throw new MidiStreamException(MidiException.TrackMissing);
            }

            return MidiTrack.CreateTrack(GetEvents());
        }

        /// <summary>
        ///  reads all events within the track.
        /// </summary>
        /// <returns>events within the track.</returns>
        private IEnumerable<IMidiEvent<MidiMessage>> GetEvents()
        {
            long totalTicks = 0L;
            var length = Reverse(_reader.ReadInt32Sync()); // bytes to read
            var pos = _reader.Position;

            while (_reader.Position - pos < length)
            {
                yield return ReadEvent(ref totalTicks);
            }
        }

        private IMidiEvent<MidiMessage> ReadEvent(ref long totalTicks)
        {
            totalTicks += _reader.Read7BitEncodedIntSync();

            // parse message
            var status = _reader.ReadByteSync();
            bool omitted = (status & 0x80) == 0;
            if (omitted)
            {
                status = _currentStatus;
                _reader.SeekSync(-1, SeekOrigin.Current); // step back
            }
            else
                _currentStatus = status;

            byte[] data;


            if (status >= 0x80 && status <= 0xEF) // voice message. most common
            {
                data = new byte[] { status, _reader.ReadByteSync(), 0, 0 };

                if (status < 0xC0 || status > 0xDF)
                {
                    data[2] = _reader.ReadByteSync();
                }

                return new MidiEvent<VoiceMessage>(totalTicks, CreateMessage<VoiceMessage>(data));
            }
            else if (status == 0xFF) // meta message. 
            {
                //if (omitted) throw new MidiStreamException(MidiException.UnknownStatus);

                var type = _reader.ReadByteSync();
                var length = _reader.ReadByteSync();
                var content = _reader.ReadBytesSync(length);
                data = new[] { status, type, length }.Concat(content).ToArray();
                return new MidiEvent<MetaMessage>(totalTicks, CreateMessage<MetaMessage>(data));
            }
            else if (status >= 0xF0 && status <= 0xF6) // system common message
            {
                if (status == 0xF0) // system exclusive message
                {
                    var id = _reader.ReadByteSync();
                    var pos = _reader.Position;
                    while (_reader.ReadByteSync() != 0x7F) { } // todo optimize
                    var newpos = _reader.Position;
                    var length = newpos - pos;
                    _reader.SeekSync(-length, SeekOrigin.Current);
                    data = new[] { status, id }.Concat(_reader.ReadBytesSync((int)length)).ToArray();
                    return new MidiEvent<SysexMessage>(totalTicks, CreateMessage<SysexMessage>(data));
                }

                switch ((SystemCommon)status)
                {
                    case SystemCommon.MidiTimeCodeQuarterFrame:
                    case SystemCommon.SongSelect:
                        data = new[] { status, _reader.ReadByteSync() };
                        break;
                    case SystemCommon.SongPositionPointer:
                        data = new[] { status, _reader.ReadByteSync(), _reader.ReadByteSync() };
                        break;
                    case SystemCommon.TuneRequest:
                        data = new[] { status };
                        break;
                    default:
                        throw new MidiStreamException(MidiException.NotSupported);
                }

                return new MidiEvent<SysCommonMessage>(totalTicks, CreateMessage<SysCommonMessage>(data));
            }
            else if (status >= 0xF8 && status <= 0xFF) // system realtime
            {
                if (omitted) throw new MidiStreamException(MidiException.UnknownStatus);
                throw new MidiStreamException(MidiException.InvalidMessage);
                //data = new[] {status};
                //return new MidiEvent<SysRealtimeMessage>(totalTicks, CreateMessage<SysRealtimeMessage>(data));
            }

            throw new MidiStreamException(MidiException.NotSupported);
        }
    }
}
