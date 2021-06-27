using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RedNX.Process.Unix;
using RedNX.Process.Windows;

namespace RedNX.Process {
    public class RedProcess {

        public int Pid { get; internal set; } = -1;
        public string Filename { get; internal set; }
        public State State { get; internal set; }
        public int ParentPid { get; internal set; } = -1;
        public int Group { get; internal set; }
        public int Session { get; internal set; }
        public int Tty { get; internal set; }
        public int TtyGroup { get; internal set; }
        public int Priority { get; internal set; }
        public int NumThreads{ get; internal set; }
        public int ExitCode{ get; internal set; }
        public long UsedMemory { get; internal set; }
        public string[] Arguments { get; internal set; }

        public async Task<RedProcess> GetParentProcess() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)) return await UnixProcessManager.GetProcess(ParentPid);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return WindowsProcessManager.GetProcess(ParentPid);
            throw new PlatformNotSupportedException();
        }

        #region Static

        public static async Task<RedProcess> GetCurrentProcess() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)) return await UnixProcessManager.GetProcess(System.Diagnostics.Process.GetCurrentProcess().Id);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return WindowsProcessManager.GetProcess(System.Diagnostics.Process.GetCurrentProcess().Id);
            throw new PlatformNotSupportedException();
        }

        public static async Task<RedProcess> GetProcessByPid(int pid) {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)) return await UnixProcessManager.GetProcess(pid);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return WindowsProcessManager.GetProcess(pid);
            throw new PlatformNotSupportedException();
        }

        #endregion

    }
}
