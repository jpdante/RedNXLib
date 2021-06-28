using System;
using System.Threading.Tasks;
using RedNX.Application;
using RedNX.Net;

namespace RedNx.Test {
    public class Program : ConsoleApplication {

        public static void Main(string[] args) {
            new Program().Run(args);
        }

        protected override Task OnLoad() {
            return Task.CompletedTask;
        }

        protected override async Task OnStart() {
            var rest = new RedJsonRest("https://www.dnd5eapi.co/api/");
            var response = await rest.Get("classes");
            Console.WriteLine(response.RootElement.ToString());
        }

        protected override Task OnExit() {
            return Task.CompletedTask;
        }
    }
}
