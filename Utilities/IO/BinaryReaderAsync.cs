using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
// ReSharper disable All

namespace Utilities.IO
{
    public sealed class BinaryReaderAsync : IDisposable
    {
        private readonly Stream _stream;
        private readonly Encoding _encoding;

        public long Position => _stream.Position - (_buffer.Length - _pos);
        public bool EndOfStream => Position == _stream.Length;

        private byte[] _buffer;
        private int _pos;

        public async Task<long> Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Current:
                    if (Position + offset > _stream.Length) offset = _stream.Length;
                    if (Position + offset < 0) offset = _stream.Length;

                    if (_pos + offset < 0 || _pos + offset > _buffer.Length)
                    {
                        var seek = offset + (_pos - _buffer.Length); //seek to actual position
                        _stream.Seek(seek, origin);
                        await PrepareBuffer(true);
                    }
                    else
                    {
                        _pos += (int) offset;
                    }
                    break;
                case SeekOrigin.Begin:
                    if (offset < 0) offset = 0;
                    if (Position > _buffer.Length || offset > _buffer.Length)
                    {
                        _stream.Seek(offset, origin);
                        await PrepareBuffer(true);
                    }
                    else
                    {
                        _pos = (int)offset;
                    }
                    break;
                case SeekOrigin.End:
                    if (offset > _stream.Length) offset = _stream.Length;
                    if (Position < _stream.Length - _buffer.Length || offset < -_buffer.Length)
                    {
                        _stream.Seek(offset, origin);
                        await PrepareBuffer(true);
                    }
                    else
                    {
                        _pos = (int)(_buffer.Length - -offset);
                    }
                    break;
                default:
                    throw new InvalidOperationException("Invalid seek origin was passed.");
            }

            return Position;
        }

        private async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token = default(CancellationToken))
        {
            return await _stream.ReadAsync(buffer, offset, count, token).ConfigureAwait(false);
        }

        private async Task PrepareBuffer(bool flush = false, CancellationToken token = default(CancellationToken))
        {
            if (flush) _pos = _buffer.Length;
            if (_pos >= _buffer.Length)
            {
                var newlen = await ReadAsync(_buffer, 0, _buffer.Length, token);
                if (newlen != _buffer.Length)
                {
                    var newbuffer = new byte[newlen];
                    Buffer.BlockCopy(_buffer, 0, newbuffer, 0, newlen);
                    _buffer = newbuffer;
                    _pos = _buffer.Length;
                    if (_buffer.Length == 0) return;
                }
                _pos %= _buffer.Length;
            }
        }
        
        private async Task<byte[]> GetBuffer(int bytesToRead, CancellationToken token = default(CancellationToken))
        {
            if (bytesToRead == 0)
                return Array.Empty<byte>();
            if(EndOfStream)
                throw new EndOfStreamException();
            if (bytesToRead < 0)
                throw new ArgumentOutOfRangeException(nameof(bytesToRead), "argument cant be negative.");

            await PrepareBuffer();

            byte[] buffer;
            var remaining = _buffer.Length - _pos;
            if (bytesToRead <= remaining)
            {
                buffer = new byte[bytesToRead];
                Buffer.BlockCopy(_buffer, _pos, buffer, 0, bytesToRead);
                _pos += bytesToRead;
            }
            else //if (bytesToRead > remaining)
            {
                buffer = new byte[bytesToRead];
                Buffer.BlockCopy(_buffer, _pos, buffer, 0, remaining);
                var read = await ReadAsync(buffer, remaining, bytesToRead - remaining, token);
                await PrepareBuffer(true);

                if (read != bytesToRead - remaining)
                {
                    var newbuffer = new byte[read + remaining];
                    Buffer.BlockCopy(buffer, 0, newbuffer, 0, newbuffer.Length);
                    buffer = newbuffer;
                }
            }

            return buffer;
        }

        public BinaryReaderAsync([NotNull] Stream input) : this(input, Encoding.UTF8)
        {
        }

        public BinaryReaderAsync([NotNull] Stream input, [NotNull] Encoding encoding) : this(input, encoding, 4096)
        {
        }

        public BinaryReaderAsync([NotNull] Stream input, [NotNull] Encoding encoding, int bufferSize)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException(nameof(bufferSize));
            if (!input.CanSeek) throw new NotSupportedException("Stream must be seekable.");
            if (!input.CanRead) throw new NotSupportedException("Stream must be readable.");

            _buffer = new byte[bufferSize];
            _pos = bufferSize; // buffer is empty
            _stream = input;
            _encoding = encoding;
        }

        public async Task<bool> ReadBoolean(CancellationToken token = default(CancellationToken))
        {
            return await ReadByte(token) != 0;
        }

        public async Task<byte> ReadByte(CancellationToken token = default(CancellationToken))
        {
            return (await ReadBytes(sizeof(byte), token))[0];
        }

        public async Task<sbyte> ReadSByte(CancellationToken token = default(CancellationToken))
        {
            return (sbyte) await ReadByte(token);
        }

        public async Task<byte[]> ReadBytes(int count, CancellationToken token = default(CancellationToken))
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            return await GetBuffer(count, token);
        }

        public async Task<short> ReadInt16(CancellationToken token = default(CancellationToken))
        {
            var buffer = await ReadBytes(sizeof(short), token);
            unsafe
            {
                fixed (byte* ptr = &buffer[0]) return *(short*) ptr;
            }
        }

        public async Task<int> ReadInt32(CancellationToken token = default(CancellationToken))
        {
            var buffer = await ReadBytes(sizeof(int), token);
            unsafe
            {
                fixed (byte* ptr = &buffer[0]) return *(int*)ptr;
            }
        }

        public async Task<long> ReadInt64(CancellationToken token = default(CancellationToken))
        {
            var buffer = await ReadBytes(sizeof(long), token);
            unsafe
            {
                fixed (byte* ptr = &buffer[0]) return *(long*)ptr;
            }
        }

        public async Task<ushort> ReadUInt16(CancellationToken token = default(CancellationToken))
        {
            return (ushort) await ReadInt16(token);
        }

        public async Task<uint> ReadUInt32(CancellationToken token = default(CancellationToken))
        {
            return (uint) await ReadInt32(token);
        }

        public async Task<ulong> ReadUInt64(CancellationToken token = default(CancellationToken))
        {
            return (ulong) await ReadInt64(token);
        }

        public async Task<float> ReadSingle(CancellationToken token = default(CancellationToken))
        {
            var num = await ReadInt32(token);
            unsafe
            {
                return *(float*) &num;
            }
        }

        public async Task<double> ReadDouble(CancellationToken token = default(CancellationToken))
        {
            var num = await ReadInt64(token);
            unsafe
            {
                return *(double*)&num;
            }
        }

        public async Task<decimal> ReadDecimal(CancellationToken token = default(CancellationToken))
        {
            var num1 = await ReadInt32(token);
            var num2 = await ReadInt32(token);
            var num3 = await ReadInt32(token);
            var num4 = await ReadInt32(token);
            return new decimal(new[] {num1, num2, num3, num4});
        }

        public async Task<char> ReadChar(CancellationToken token = default(CancellationToken))
        {
            return (await ReadChars(1, token))[0];
        }

        [ItemNotNull]
        public async Task<char[]> ReadChars(int count, CancellationToken token = default(CancellationToken))
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            var max = _encoding.GetMaxByteCount(count);
            var buffer = await GetBuffer(max, token);
            var chars = _encoding.GetChars(buffer);
            var bytes = _encoding.GetByteCount(chars, 0, count);
            var moveback = bytes - buffer.Length;

            if (moveback < 0)
            {
                await Seek(moveback, SeekOrigin.Current);
            }
            if (chars.Length > count)
            {
                var newchars = new char[count];
                Buffer.BlockCopy(chars, 0, newchars, 0, count*2);
                chars = newchars;
            }

            return chars;
        }

        [ItemNotNull]
        public async Task<string> ReadString(CancellationToken token = default(CancellationToken))
        {
            var count = await Read7BitEncodedInt(token);
            var chars = await ReadChars(count, token);
            return new string(chars);
        }

        public async Task<int> Read7BitEncodedInt(CancellationToken token = default(CancellationToken))
        {
            long value = 0;
            int b;
            do
            {
                b = await ReadByte(token);
                value = checked((uint)((value << 7) | (b & 0x7FL)));
            } while ((b & 0x80) != 0);
            return unchecked ((int) value);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
