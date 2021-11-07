using System.IO;

namespace RedNX.Caching {
    public class FileCacheConfig {
        public string CachePath { get; set; } = null;

        public string DatabaseFileName { get; set; } = null;

        public long MaxAvailableSpace { get; set; } = 512 * 1024 * 1024; // 512 MB

        public bool ReleaseWhenAddingIfFull { get; set; } = true;

        public bool ReleaseIfFullAtInit { get; set; } = true;

        public bool DeleteUnknownFilesAtInit { get; set; } = false;

        public bool RemoveMissingFilesAtInit { get; set; } = true;

        public ReleaseMode DefaultReleaseMode { get; set; } = ReleaseMode.OldestAccessDate;

        public FileCacheConfig() {
        }

        public FileCacheConfig(string path) {
            CachePath = Path.GetFullPath(path);
            DatabaseFileName = Path.Combine(CachePath, "caching.db");
        }
    }
}