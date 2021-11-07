using System;

namespace RedNX.Caching.Exceptions {
    public class FileCacheException : Exception {
        public FileCacheException(string message) : base(message) {
        }
    }
}