using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RedNX.Application;
using RedNX.Process;

namespace RedNx.Test {
    public class Program : ConsoleApplication {

        public static void Main(string[] args) {
            new Program().Run(args);
        }

        protected override Task OnLoad() {
            return Task.CompletedTask;
        }

        protected override async Task OnStart() {
            var process = await RedProcess.GetCurrentProcess();
            Console.WriteLine(process.Pid);
            Console.WriteLine(process.Filename);
            Console.WriteLine(process.State);
            Console.WriteLine(process.ParentPid);
            Console.WriteLine(process.Group);
            Console.WriteLine(process.Session);
            Console.WriteLine(process.Tty);
            Console.WriteLine(process.TtyGroup);
            Console.WriteLine(process.Priority);
            Console.WriteLine(process.NumThreads);
            Console.WriteLine(process.ExitCode);
            Console.WriteLine(process.UsedMemory);
            foreach (string arg in process.Arguments) {
                Console.Write($"{arg} ");
            }
        }

        protected override Task OnExit() {
            return Task.CompletedTask;
        }
    }
}
