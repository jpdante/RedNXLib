using System;
using System.Threading.Tasks;

namespace RedNX.Process.Unix {
    internal class UnixProcessManager {

        public static async Task<RedProcess> GetProcess(int pid) {
            if (pid < 0) throw new IndexOutOfRangeException();
            var reader = new UnixProcessReader(pid);
            string[] status = await reader.GetStatus();
            string[] memory = await reader.GetMemoryStatus();
            string[] arguments = await reader.GetCmdArguments();
            return new RedProcess {
                Pid = int.Parse(status[0]),
                Filename = status[1].Remove(0, 1).Remove(status[1].Length - 2, 1),
                State = GetState(status[2]),
                ParentPid = int.Parse(status[3]),
                Group = int.Parse(status[4]),
                Session = int.Parse(status[5]),
                Tty = int.Parse(status[6]),
                TtyGroup = int.Parse(status[7]),
                Priority = int.Parse(status[17]),
                NumThreads = int.Parse(status[19]),
                ExitCode = int.Parse(status[51]),
                UsedMemory = long.Parse(memory[0]),
                Arguments = arguments
            };
        }

        private static State GetState(string data) {
            return data switch {
                "R" => State.Running,
                "S" => State.Sleeping,
                "D" => State.Waiting,
                "Z" => State.Zombie,
                "T" => State.Stopped,
                "t" => State.Tracing,
                "X" => State.Dead,
                "x" => State.Dead,
                _ => State.Unknown
            };
        }

    }
}
