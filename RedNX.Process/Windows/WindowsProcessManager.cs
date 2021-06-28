using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace RedNX.Process.Windows {
    public class WindowsProcessManager {

        public static RedProcess GetProcess(int pid) {
            if (pid < 0) throw new IndexOutOfRangeException();
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return null;
            using var searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_Process WHERE ProcessId = {pid}");
            using var objects = searcher.Get();
            if (objects.Count == 0) throw new KeyNotFoundException();

            var arguments = objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
            string[] argumentList = !string.IsNullOrEmpty(arguments) ? objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString()?.Split(' ') : new string[0];

            return new RedProcess {
                Pid = int.Parse(objects.Cast<ManagementBaseObject>().SingleOrDefault()?["ProcessId"].ToString() ?? "-1"),
                Filename = objects.Cast<ManagementBaseObject>().SingleOrDefault()?["Name"].ToString(),
                State = State.Running,
                ParentPid = int.Parse(objects.Cast<ManagementBaseObject>().SingleOrDefault()?["ParentProcessId"].ToString() ?? "-1"),
                Group = -1,
                Session = int.Parse(objects.Cast<ManagementBaseObject>().SingleOrDefault()?["SessionId"].ToString() ?? "-1"),
                Tty = -1,
                TtyGroup = -1,
                Priority = int.Parse(objects.Cast<ManagementBaseObject>().SingleOrDefault()?["Priority"].ToString() ?? "0"),
                NumThreads = int.Parse(objects.Cast<ManagementBaseObject>().SingleOrDefault()?["ThreadCount"].ToString() ?? "0"),
                ExitCode = 0,
                UsedMemory = long.Parse(objects.Cast<ManagementBaseObject>().SingleOrDefault()?["WorkingSetSize"].ToString() ?? "0"),
                Arguments = argumentList
            };
        }
    }
}