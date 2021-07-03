using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RedNX.Application;
using RedNX.Net.Protocol.ProtoRed;
using RedNX.Net.Socket;

namespace RedNx.Test {
    public class Program : ConsoleApplication {

        public RedTcpClient Client;
        public RedTcpServer Server;

        public RedTcpClient TempClient;

        private Serializer _serializer;

        public static void Main(string[] args) {
            new Program().Run(args);
        }

        protected override async Task OnLoad() {
            _serializer = new Serializer();
            _serializer.AddType(typeof(TestClass));
            _serializer.AddType(typeof(TestSubClass));

            Console.WriteLine("[Server] Starting...");
            Server = new RedTcpServer(new IPEndPoint(IPAddress.Any, 3005));
            Server.Start();
            Server.AuthorizesIP += ServerOnAuthorizesIP;
            Server.NewClientConnected += ServerOnNewClientConnected;
            Console.WriteLine("[Server] Started!");

            Console.WriteLine("[Client] Connecting...");
            Client = new RedTcpClient();
            await Client.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3005));
            Console.WriteLine("[Client] Connected!");
        }

        private async void ServerOnNewClientConnected(object sender, RedTcpClient client) {
            Console.WriteLine("[Server] New client!");
            TempClient = client;


            Console.WriteLine("[Client] Sending data...");

            await _serializer.Serialize(new TestClass(), Client.Stream);
            await Client.Stream.FlushAsync();
            Console.WriteLine("[Client] Data sent!");

            var data = await _serializer.Deserialize<TestClass>(TempClient.Stream);
            Console.WriteLine(data.Test0);

        }

        private void ServerOnAuthorizesIP(object sender, AuthorizesIPEventArgs e) {
            e.Authorize = true;
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

        protected override async Task OnExit() {
            await Client.DisconnectAsync();
            Server.Stop();
            Console.WriteLine("Exiting...");
        }
    }
}
