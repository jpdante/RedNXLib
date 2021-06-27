using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace RedNX.Process.Windows {
    public class WindowsProcessManager {

        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);

        public static RedProcess GetProcess(int pid) {
            if (pid < 0) throw new IndexOutOfRangeException();
            var wProcess = System.Diagnostics.Process.GetProcessById(pid);
            return new RedProcess {
                Pid = wProcess.Id,
                Filename = wProcess.StartInfo.FileName,
                State = GetState(wProcess),
                ParentPid = GetParentProcessPid(wProcess.Handle),
                Group = -1,
                Session = -1,
                Tty = -1,
                TtyGroup = -1,
                Priority = wProcess.BasePriority,
                NumThreads = wProcess.Threads.Count,
                ExitCode = wProcess.ExitCode,
                UsedMemory = wProcess.WorkingSet64,
                Arguments = wProcess.StartInfo.ArgumentList.ToArray()
            };
        }

        private static State GetState(System.Diagnostics.Process process) {
            return process.HasExited ? State.Stopped : State.Running;
        }

        public static int GetParentProcessPid(IntPtr handle) {
            var pbi = new ParentProcessUtilities();
            int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out int _);
            if (status != 0)
                throw new Win32Exception(status);
            try {
                return pbi.InheritedFromUniqueProcessId.ToInt32();
            } catch (ArgumentException) {
                return -1;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ParentProcessUtilities {
            internal IntPtr Reserved1;
            internal IntPtr PebBaseAddress;
            internal IntPtr Reserved2_0;
            internal IntPtr Reserved2_1;
            internal IntPtr UniqueProcessId;
            internal IntPtr InheritedFromUniqueProcessId;
        }
    }
}