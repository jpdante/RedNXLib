using System.IO;
using RedNX.IO.Exceptions;

namespace RedNX.IO.Streams {
    public class ReadOnlyStream : Stream {

        private readonly Stream _stream;

        public ReadOnlyStream(Stream stream) {
            _stream = stream;
        }

        public override void Flush() {
            throw new ReadOnlyStreamException("It is not possible to flush a read-only stream.");
        }

        public override int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) {
            throw new ReadOnlyStreamException("It is not possible to seek a read-only stream.");
        }

        public override void SetLength(long value) {
            throw new ReadOnlyStreamException("It is not possible to set a length for a read-only stream.");
        }

        public override void Write(byte[] buffer, int offset, int count) {
            throw new ReadOnlyStreamException("It is not possible to write to a read-only stream.");
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _stream.Length;
        public override long Position {
            get => _stream.Position;
            set => throw new ReadOnlyStreamException("It is not possible to set position to a read-only stream.");
        }
    }
}