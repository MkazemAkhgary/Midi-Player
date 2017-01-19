using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Utilities.IO;

// ReSharper disable MemberCanBePrivate.Global

namespace MidiStream
{
    /// <summary>
    /// Initializes a new instance of <see cref="MidiStreamReader"/> with specified path.
    /// </summary>
    public sealed partial class MidiStreamReader : IDisposable
    {
        private readonly BinaryReaderAsync _reader;
        private MidiStream _stream;

        private long _totalTicks;
        private byte _currentStatus;

        [NotNull]
        public string FullPath { get; }
        
        public MidiStreamReader([NotNull] string path, [NotNull] Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(path));
            if(!File.Exists(path))
                throw new FileNotFoundException(@"File does not exist.", nameof(path));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));

            FullPath = path;
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            _reader = new BinaryReaderAsync(stream, encoding);
        }

        public MidiStreamReader([NotNull] string path) : this(path, Encoding.ASCII)
        {
        }

        /// <summary>
        /// gets the parsed midi stream.
        /// </summary>
        [NotNull]
        public MidiStream GetStream()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
