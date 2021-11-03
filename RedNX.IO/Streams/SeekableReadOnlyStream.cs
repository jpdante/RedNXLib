using System.IO;
using RedNX.IO.Exceptions;

namespace RedNX.IO.Streams {
    public class SeekableReadOnlyStream : Stream {

        private readonly Stream _stream;

        public SeekableReadOnlyStream(Stream stream) {
            _stream = stream;
        }

        public override void Flush() {
            throw new ReadOnlyStreamException("It is not possible to flush a read-only stream.");
        }

        public override int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);

        public override void SetLength(long value) {
            throw new ReadOnlyStreamException("It is not possible to set a length for a read-only stream.");
        }

        public override void Write(byte[] buffer, int offset, int count) {
            throw new ReadOnlyStreamException("It is not possible to write to a read-only stream.");
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => _stream.Length;
        public override long Position {
            get => _stream.Position;
            set => _stream.Position = value;
        }
    }
}