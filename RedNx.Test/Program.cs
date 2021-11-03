using System;
using System.Threading.Tasks;
using RedNX.Application;

namespace RedNx.Test {
    public class Program : ConsoleApplication {

        public static void Main(string[] args) {
            new Program().Run(args);
        }

        protected override Task OnLoad() {
            return Task.CompletedTask;
        }

        protected override Task OnStart() {
            while (true) {
                Console.Write("Exit? ");
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;
                switch (line) {
                    case "y":
                    case "Y":
                    case "s":
                    case "S":
                        Stop();
                        return Task.CompletedTask;
                }
            }
        }

        protected override Task OnExit() {
            Console.WriteLine("Exiting...");
            return Task.CompletedTask;
        }
    }
}
