using System.IO;
using RedNX.IO.Exceptions;

namespace RedNX.IO.Streams {
    public class WriteOnlyStream : Stream {

        private readonly Stream _stream;

        public WriteOnlyStream(Stream stream) {
            _stream = stream;
        }

        public override void Flush() => _stream.Flush();

        public override int Read(byte[] buffer, int offset, int count) {
            throw new WriteOnlyStreamException("It is not possible to read a write-only stream.");
        }

        public override long Seek(long offset, SeekOrigin origin) {
            throw new WriteOnlyStreamException("It is not possible to seek a write-only stream.");
        }

        public override void SetLength(long value) {
            throw new WriteOnlyStreamException("It is not possible to set a length for a write-only stream.");
        }

        public override void Write(byte[] buffer, int offset, int count) => _stream.Write(buffer, offset, count);

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => _stream.Length;
        public override long Position {
            get => _stream.Position;
            set => throw new WriteOnlyStreamException("It is not possible to set position to a write-only stream.");
        }
    }
}