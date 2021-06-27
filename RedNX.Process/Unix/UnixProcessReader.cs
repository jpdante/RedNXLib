using System.IO;
using System.Threading.Tasks;

namespace RedNX.Process.Unix {
    public class UnixProcessReader {

        private readonly int _pid;
        private readonly string _statusPath;
        private readonly string _memoryStatusPath;
        private readonly string _cmdArgumentsPath;

        public UnixProcessReader(int pid) {
            _pid = pid;
            string basePath = Path.Combine(@"/proc/", pid.ToString());
            _statusPath = Path.Combine(basePath, "stat");
            _memoryStatusPath = Path.Combine(basePath, "statm");
            _cmdArgumentsPath = Path.Combine(basePath, "cmdline");
        }

        public async Task<string[]> GetStatus() {
            await using var fs = new FileStream(_statusPath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs);
            string data = await sr.ReadToEndAsync();
            if (string.IsNullOrEmpty(data)) throw new EndOfStreamException($"Failed to get process <{_pid}> stat.");
            return data.Split(' ');
        }

        public async Task<string[]> GetMemoryStatus() {
            await using var fs = new FileStream(_memoryStatusPath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs);
            string data = await sr.ReadToEndAsync();
            if (string.IsNullOrEmpty(data)) throw new EndOfStreamException($"Failed to get process <{_pid}> statm.");
            return data.Split(' ');
        }

        public async Task<string[]> GetCmdArguments() {
            await using var fs = new FileStream(_cmdArgumentsPath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs);
            string data = await sr.ReadToEndAsync();
            if (string.IsNullOrEmpty(data)) throw new EndOfStreamException($"Failed to get process <{_pid}> cmdline.");
            return data.Split('\0');
        }

    }
}
