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

        protected override async Task OnStart() {

        }

        protected override Task OnExit() {
            return Task.CompletedTask;
        }
    }
}
