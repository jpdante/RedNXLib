using System;
using RedNX.Extensions.System;

namespace RedNX.Caching.Internal {
    internal class FileEntry {
        internal string Id { get; private set; }
        internal string FileName { get; private set; }
        internal long Size { get; private set; }
        internal int CreationDate { get; private set; }
        internal int LastAccess { get; private set; }

        public FileEntry(string id, string fileName, long size, DateTime creationDate, DateTime lastAccess) {
            Id = id;
            FileName = fileName;
            Size = size;
            CreationDate = creationDate.ToUnixEpoch();
            LastAccess = lastAccess.ToUnixEpoch();
        }

        public FileEntry(string id, string fileName, long size, int creationDate, int lastAccess) {
            Id = id;
            FileName = fileName;
            Size = size;
            CreationDate = creationDate;
            LastAccess = lastAccess;
        }
    }
}