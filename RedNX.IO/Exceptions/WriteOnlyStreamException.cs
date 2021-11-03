using System;

namespace RedNX.IO.Exceptions {
    public class WriteOnlyStreamException : Exception {

        public WriteOnlyStreamException(string message) : base(message) {

        }

    }
}