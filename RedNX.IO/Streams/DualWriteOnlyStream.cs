using System.IO;
using RedNX.IO.Exceptions;

namespace RedNX.IO.Streams {
    public class DualWriteOnlyStream : Stream {

        private readonly Stream _stream1;
        private readonly Stream _stream2;

        public DualWriteOnlyStream(Stream stream1, Stream stream2) {
            _stream1 = stream1;
            _stream2 = stream2;
        }

        public override void Flush() {
            _stream1.Flush();
            _stream2.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count) {
            throw new WriteOnlyStreamException("It is not possible to read a dual write-only stream.");
        }

        public override long Seek(long offset, SeekOrigin origin) {
            throw new WriteOnlyStreamException("It is not possible to seek a dual write-only stream.");
        }

        public override void SetLength(long value) {
            throw new WriteOnlyStreamException("It is not possible to set a length for a dual write-only stream.");
        }

        public override void Write(byte[] buffer, int offset, int count) {
            _stream1.Write(buffer, offset, count);
            _stream2.Write(buffer, offset, count);
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => -1;
        public override long Position {
            get => throw new WriteOnlyStreamException("It is not possible to get position to a dual write-only stream.");
            set => throw new WriteOnlyStreamException("It is not possible to set position to a dual write-only stream.");
        }
    }
}