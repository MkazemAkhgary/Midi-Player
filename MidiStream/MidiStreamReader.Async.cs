using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        [ItemNotNull]
        public async Task<MidiStream> GetStreamAsync()
        {
            if (_stream != null) return _stream;

            var name = Path.GetFileNameWithoutExtension(FullPath);
             
            // read header
            var header = await _reader.ReadChars(4);
            var length = Reverse(await _reader.ReadInt32());
            var type = Reverse(await _reader.ReadInt16());
            var numberOfTracks = Reverse(await _reader.ReadInt16());
            var timedivision = Reverse(await _reader.ReadInt16());

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
                tracks[i] = await ReadTrackAsync();
            }

            var format = new MidiFormat((MidiType) type, timedivision);

            return _stream = new MidiStream(tracks, format, name);
        }

        private async Task<MidiTrack> ReadTrackAsync()
        {
            var header = await _reader.ReadChars(4);
            if (!header.SequenceEqual("MTrk")) // check header 
            {
                throw new MidiStreamException(MidiException.TrackMissing);
            }
            
            return new MidiTrack(await GetEventsAsync());
        }

        /// <summary>
        ///  reads all events within the track.
        /// </summary>
        /// <returns>events within the track.</returns>
        private async Task<IEnumerable<IMidiEvent<MidiMessage>>> GetEventsAsync()
        {
            _totalTicks = 0;
            var list = new List<IMidiEvent<MidiMessage>>();

            var length = Reverse(await _reader.ReadInt32()); // bytes to read
            var pos = _reader.Position;

            while (_reader.Position - pos < length)
            {
                list.Add(await ReadEvent());
            }
            return list;
        }

        private async Task<IMidiEvent<MidiMessage>> ReadEvent()
        {
            _totalTicks += await _reader.Read7BitEncodedInt();

            // parse message
            var status = await _reader.ReadByte();
            bool omitted = (status & 0x80) == 0;
            if (omitted)
            {
                status = _currentStatus;
                await _reader.Seek(-1, SeekOrigin.Current); // step back
            }
            else
                _currentStatus = status;
            
            byte[] data;


            if (status >= 0x80 && status <= 0xEF) // voice message. most common
            {
                data = new byte[] {status, await _reader.ReadByte(), 0, 0};

                if (status < 0xC0 || status > 0xDF)
                {
                    data[2] = await _reader.ReadByte();
                }

                return new MidiEvent<VoiceMessage>(_totalTicks, CreateMessage<VoiceMessage>(data));
            }
            else if (status == 0xFF) // meta message. 
            {
                //if (omitted) throw new MidiStreamException(MidiException.UnknownStatus);

                var type = await _reader.ReadByte();
                var length = await _reader.ReadByte();
                var content = await _reader.ReadBytes(length);
                data = new[] { status, type, length }.Concat(content).ToArray();
                return new MidiEvent<MetaMessage>(_totalTicks, CreateMessage<MetaMessage>(data));
            }
            else if(status >= 0xF0 && status <= 0xF6) // system common message
            {
                if (status == 0xF0) // system exclusive message
                {
                    var id = await _reader.ReadByte();
                    var pos = _reader.Position;
                    while (await _reader.ReadByte() != 0x7F) { } // todo optimize
                    var newpos = _reader.Position;
                    var length = newpos - pos;
                    await _reader.Seek(-length, SeekOrigin.Current);
                    data = new[] {status, id}.Concat(await _reader.ReadBytes((int) length)).ToArray();
                    return new MidiEvent<SysexMessage>(_totalTicks, CreateMessage<SysexMessage>(data));
                }

                switch ((SystemCommon)status)
                {
                    case SystemCommon.MidiTimeCodeQuarterFrame:
                    case SystemCommon.SongSelect:
                        data = new[] {status, await _reader.ReadByte()};
                        break;
                    case SystemCommon.SongPositionPointer:
                        data = new[] {status, await _reader.ReadByte(), await _reader.ReadByte()};
                        break;
                    case SystemCommon.TuneRequest:
                        data = new[] {status};
                        break;
                    default:
                        throw new MidiStreamException(MidiException.NotSupported);
                }

                return new MidiEvent<SysCommonMessage>(_totalTicks, CreateMessage<SysCommonMessage>(data));
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
