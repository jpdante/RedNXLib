using System;
using System.Threading;
using System.Threading.Tasks;

namespace RedNX.Application {
    public abstract class DaemonApplication {

        private readonly ManualResetEvent _shutdownEvent;
        protected string[] Args { get; private set; }

        protected DaemonApplication() {
            _shutdownEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += ConsoleOnCancelKeyPress;
        }

        public void Run(string[] args) {
            Args = args;
            _shutdownEvent.Reset();
            try {
                var task = AsyncRun();
                task.Wait();
            } catch (AggregateException ex) {
                if(ex.InnerException != null) throw ex.InnerException;
            }
        }

        private async Task AsyncRun() {
            await OnLoad();
            await OnStart();
            _shutdownEvent.WaitOne();
            await OnExit();
        }

        private void ConsoleOnCancelKeyPress(object? sender, ConsoleCancelEventArgs e) {
            _shutdownEvent.Set();
            e.Cancel = true;
        }

        public void Stop() {
            _shutdownEvent.Set();
        }

        protected abstract Task OnLoad();

        protected abstract Task OnStart();

        protected abstract Task OnExit();

    }
}