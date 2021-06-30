using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using RedNX.Application;
using RedNX.Net;

namespace RedNx.Test {
    public class Program : ConsoleApplication {

        public static void Main(string[] args) {
            new Program().Run(args);
        }

        protected override async Task OnLoad() {
            var rest = new RedJsonRest("https://www.dnd5eapi.co/api/");
            var response = await rest.Get("classes");
            Console.WriteLine(response.RootElement.ToString());
            Console.WriteLine();
        }

        protected override Task OnStart() {
            while (true) {
                Console.Write("Exit? ");
                string line = Console.ReadLine();
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
