#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RedNX.Caching.Exceptions;
using RedNX.Caching.Internal;
using RedNX.IO.Streams;

namespace RedNX.Caching {
    public class FileCache {
        private readonly string _cachePath;
        private readonly bool _releaseWhenAddingIfFull;
        private readonly bool _releaseIfFullAtInit;
        private readonly bool _deleteUnknownFilesAtInit;
        private readonly bool _removeMissingFilesAtInit;
        private readonly ReleaseMode _defaultReleaseMode;
        private readonly CachingDatabase _cachingDatabase;

        public readonly long MaxAvailableSpace;
        public long UsedSpace { get; private set; }
        public long AvailableSpace => MaxAvailableSpace - UsedSpace;

        public FileCache(FileCacheConfig config) {
            _cachePath = config.CachePath;
            _releaseWhenAddingIfFull = config.ReleaseWhenAddingIfFull;
            _defaultReleaseMode = config.DefaultReleaseMode;
            _deleteUnknownFilesAtInit = config.DeleteUnknownFilesAtInit;
            _removeMissingFilesAtInit = config.RemoveMissingFilesAtInit;
            _releaseIfFullAtInit = config.ReleaseIfFullAtInit;

            MaxAvailableSpace = config.MaxAvailableSpace;

            if (!Directory.Exists(_cachePath)) Directory.CreateDirectory(_cachePath);
            _cachingDatabase = new CachingDatabase(config.DatabaseFileName);
            Init();
        }

        public void Init() {
            IReadOnlyCollection<FileEntry>? entries = null;
            if (_deleteUnknownFilesAtInit) {
                entries = _cachingDatabase.GetEntries();
                var entriesSet = entries.Select(e => e.Id).ToHashSet();
                var files = Directory.GetFiles(_cachePath, "*.cache", SearchOption.TopDirectoryOnly);
                foreach (var file in files) {
                    var pureFileName = Path.GetFileNameWithoutExtension(file);
                    if (entriesSet.Contains(pureFileName)) continue;
                    try {
                        File.Delete(file);
                    } catch {
                        // ignored
                    }
                }
            }
            if (_removeMissingFilesAtInit) {
                entries ??= _cachingDatabase.GetEntries();
                foreach (var entry in entries) {
                    var fileName = Path.GetFullPath(Path.Combine(_cachePath, $"{entry.FileName}.cache"));
                    if (!File.Exists(fileName)) {
                        _cachingDatabase.RemoveEntry(entry.Id);
                    }
                }
            }
            UsedSpace = _cachingDatabase.SumSize();
            if (_releaseIfFullAtInit && UsedSpace > MaxAvailableSpace) {
                RequestSpaceRelease(MaxAvailableSpace - UsedSpace);
            }
        }

        public bool HasAvailableSpace(long size) => AvailableSpace >= size;

        public bool RequestSpaceRelease(long releaseSize) => RequestSpaceRelease(releaseSize, _defaultReleaseMode);

        public bool RequestSpaceRelease(long releaseSize, ReleaseMode releaseMode) {
            if (releaseSize < 1) throw new ArgumentOutOfRangeException(nameof(releaseSize), "Cannot release lower than 1.");
            if (releaseSize > MaxAvailableSpace) throw new ArgumentOutOfRangeException(nameof(releaseSize), "Cannot release larger than max available space.");
            switch (releaseMode) {
                case ReleaseMode.Random:
                    break;
                case ReleaseMode.LargestSize:
                    var largestEntries = _cachingDatabase.GetLargestSize(releaseSize);
                    foreach (var id in largestEntries) {
                        var entry = _cachingDatabase.GetEntry(id);
                        Console.WriteLine($"{id} - {entry.Size}");
                    }
                    break;
                case ReleaseMode.SmallestSize:
                    break;
                case ReleaseMode.OldestCreationDate:
                    break;
                case ReleaseMode.NewestCreationDate:
                    break;
                case ReleaseMode.OldestAccessDate:
                    break;
                case ReleaseMode.NewestAccessDate:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(releaseMode), releaseMode, null);
            }
            return false;
        }

        public bool AddFile(string identifier, long size, Action<WritableCacheStream> streamCopyFunc) {
            if (size > MaxAvailableSpace) throw new FileCacheException("Size is larger than max available space.");
            var guid = Guid.NewGuid();
            var cachedFileName = $"{guid:N}.cache";
            var cachedFilePath = Path.GetFullPath(Path.Combine(_cachePath, cachedFileName));
            try {
                using var fileStream = new FileStream(cachedFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
                streamCopyFunc.Invoke(new WritableCacheStream(fileStream, size));
                var fileEntry = new FileEntry(identifier, guid.ToString("N"), fileStream.Length, DateTime.UtcNow, DateTime.UtcNow);
                _cachingDatabase.AddEntry(fileEntry);
                UsedSpace += fileEntry.Size;
                return true;
            } catch {
                if (!File.Exists(cachedFilePath)) return false;
                try {
                    File.Delete(cachedFilePath);
                } catch {
                    // ignored
                }
                return false;
            }
        }

        public bool AddFile(string identifier, Stream dataStream) {
            if (dataStream.Length > MaxAvailableSpace) throw new FileCacheException("Stream size is larger than max available space.");
            var guid = Guid.NewGuid();
            var cachedFileName = $"{guid:N}.cache";
            var cachedFilePath = Path.GetFullPath(Path.Combine(_cachePath, cachedFileName));
            try {
                using var fileStream = new FileStream(cachedFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
                dataStream.CopyTo(fileStream);
                var fileEntry = new FileEntry(identifier, guid.ToString("N"), fileStream.Length, DateTime.UtcNow, DateTime.UtcNow);
                _cachingDatabase.AddEntry(fileEntry);
                UsedSpace += fileEntry.Size;
                return true;
            } catch {
                if (!File.Exists(cachedFilePath)) return false;
                try {
                    File.Delete(cachedFilePath);
                } catch {
                    // ignored
                }
                return false;
            }
        }

        public bool AddMoveFile(string identifier, string fileName) {
            if (ContainsFile(identifier)) return false;
            var fileInfo = new FileInfo(fileName);
            if (fileInfo == null) throw new FileNotFoundException("Failed to read file info.", fileName);
            if (fileInfo.Length > MaxAvailableSpace) throw new FileCacheException("File size is larger than max available space.");
            var guid = Guid.NewGuid();
            var cachedFileName = $"{guid:N}.cache";
            var cachedFilePath = Path.GetFullPath(Path.Combine(_cachePath, cachedFileName));
            try {
                File.Move(fileName, cachedFilePath);
                var fileEntry = new FileEntry(identifier, guid.ToString("N"), fileInfo.Length, DateTime.UtcNow, DateTime.UtcNow);
                _cachingDatabase.AddEntry(fileEntry);
                UsedSpace += fileEntry.Size;
                return true;
            } catch {
                if (!File.Exists(cachedFilePath)) return false;
                try {
                    File.Delete(cachedFilePath);
                } catch {
                    // ignored
                }
                return false;
            }
        }

        public bool GetFile(string identifier, Action<SeekableReadOnlyStream> streamCopyFunc, out Exception? actionException) {
            actionException = null;
            if (!ContainsFile(identifier)) return false;

            var fileEntry = _cachingDatabase.GetEntry(identifier);
            if (fileEntry == null) return false;

            var cachedFileName = $"{fileEntry.FileName}.cache";
            var cachedFilePath = Path.GetFullPath(Path.Combine(_cachePath, cachedFileName));

            try {
                using var fileStream = new FileStream(cachedFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                _cachingDatabase.UpdateLastAcess(identifier, DateTime.UtcNow);
                try {
                    streamCopyFunc.Invoke(new SeekableReadOnlyStream(fileStream));
                    return true;
                } catch (Exception ex) {
                    actionException = ex;
                    return false;
                }
            } catch {
                return false;
            }
        }

        public bool RemoveFile(string identifier) {
            var fileEntry = _cachingDatabase.GetEntry(identifier);
            if (fileEntry == null) return true;
            var cachedFileName = $"{fileEntry.FileName}.cache";
            var cachedFilePath = Path.GetFullPath(Path.Combine(_cachePath, cachedFileName));
            try {
                if (File.Exists(cachedFilePath)) File.Delete(cachedFilePath);
                _cachingDatabase.RemoveEntry(identifier);
                UsedSpace -= fileEntry.Size;
                return true;
            } catch {
                // ignored
            }
            return false;
        }

        public bool ContainsFile(string identifier) {
            return _cachingDatabase.ContainsEntry(identifier);
        }

        public void Clear() {
            IReadOnlyCollection<FileEntry> entries = _cachingDatabase.GetEntries();
            foreach (var entry in entries) {
                var fileName = Path.GetFullPath(Path.Combine(_cachePath, $"{entry.FileName}.cache"));
                try {
                    if (File.Exists(fileName)) File.Delete(fileName);
                } catch {
                    // ignored
                }
            }
            _cachingDatabase.Clear();
            UsedSpace = 0;
        }
    }
}