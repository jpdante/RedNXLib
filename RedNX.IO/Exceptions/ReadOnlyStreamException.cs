using System;

namespace RedNX.IO.Exceptions {
    public class ReadOnlyStreamException : Exception {

        public ReadOnlyStreamException(string message) : base(message) {

        }

    }
}