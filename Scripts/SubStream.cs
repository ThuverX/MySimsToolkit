using System;
using System.IO;

namespace MySimsToolkit.Scripts
{
    public class SubStream : Stream
    {
        private readonly Stream _baseStream;
        private readonly long _start;
        private readonly long _length;
        private long _position;

        public SubStream(Stream baseStream, long start, long length)
        {
            _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            _start = start;
            _length = length;
            _position = 0;

            _baseStream.Seek(_start, SeekOrigin.Begin);
        }

        public override bool CanRead => _baseStream.CanRead;
        public override bool CanSeek => _baseStream.CanSeek;
        public override bool CanWrite => false; // read-only
        public override long Length => _length;

        public override long Position
        {
            get => _position;
            set => Seek(value, SeekOrigin.Begin);
        }

        public override void Flush()
        {
            // read-only, nothing to flush
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_position >= _length)
                return 0; // EOF

            var toRead = (int)Math.Min(count, _length - _position);
            _baseStream.Seek(_start + _position, SeekOrigin.Begin);
            var read = _baseStream.Read(buffer, offset, toRead);
            _position += read;
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var newPos = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => _position + offset,
                SeekOrigin.End => _length + offset,
                _ => throw new ArgumentOutOfRangeException(nameof(origin), "Invalid SeekOrigin")
            };

            if (newPos < 0 || newPos > _length)
                throw new IOException("Seek outside of SubStream bounds");

            _position = newPos;
            return _position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("SubStream is read-only");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("SubStream is read-only");
        }
    }
}
