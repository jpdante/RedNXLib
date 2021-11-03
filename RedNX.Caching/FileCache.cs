using System.IO;
using RedNX.IO.Streams;

namespace RedNX.Caching {
    public class FileCache {

        public FileCache(string path) {

        }

        public bool RequestSpaceRelease(long size) {
            return false;
        }

        public long GetUsedSpace() {
            return 0;
        }

        public bool HasAvailableSpace(long size) {
            return false;
        }

        public bool AddFile(string identifier, Stream dataStream) {
            return false;
        }

        public bool RemoveFile(string identifier) {
            return false;
        }

        public bool GetFile(string identifier, out ReadOnlyStream dataStream) {
            dataStream = null;
            return false;
        }

        public bool ContainsFile(string identifier) {
            return false;
        }

        public void Clear() {

        }
    }
}
