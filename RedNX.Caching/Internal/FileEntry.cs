﻿using System;

namespace RedNX.Caching.Internal {
    internal class FileEntry {

        public string Identifier { get; }
        public long Size { get; }
        public string File { get; }
        public DateTime CreateDateTime { get; }

    }
}