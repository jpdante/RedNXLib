using System.IO;
using RedNX.Caching.Exceptions;

namespace RedNX.Caching {
    public class WritableCacheStream : Stream {

        private readonly Stream _stream;
        private readonly long _maxWriteSize;
        private long _currentWriteSize;

        internal WritableCacheStream(Stream stream, long maxWriteSize) {
            _stream = stream;
            _maxWriteSize = maxWriteSize;
            _currentWriteSize = 0;
        }

        public override void Flush() => _stream.Flush();

        public override int Read(byte[] buffer, int offset, int count) => throw new CacheStreamException("It is not possible to read a writable cache stream.");

        public override long Seek(long offset, SeekOrigin origin) => throw new CacheStreamException("It is not possible to seek a writable cache stream.");

        public override void SetLength(long value) => throw new CacheStreamException("It is not possible to set length of a writable cache stream.");

        public override void Write(byte[] buffer, int offset, int count) {
            if (_currentWriteSize + count > _maxWriteSize) throw new CacheStreamException("Maximum stream size reached.");
            _stream.Write(buffer, offset, count);
            _currentWriteSize += count;
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => _maxWriteSize;

        public override long Position {
            get => _currentWriteSize;
            set => throw new CacheStreamException("It is not possible to set position of a writable cache stream.");
        }
    }
}