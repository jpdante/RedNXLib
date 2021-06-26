using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using RedNX.Net.Socket.Internal;

namespace RedNX.Net.Socket {
    public class RedClient : IDisposable {

        private System.Net.Sockets.Socket _socket;
        private NetworkStream _networkStream;
        private SslStream _sslStream;
        private readonly ManualResetEventAsync _connectDone;
        private Exception _connectException;
        private bool _isEncrypted;
        private readonly X509Certificate _certificate;
        private readonly bool _checkCertificateRevocation;
        private string _targetHost;

        public Stream Stream { get; private set; }
        public SocketStatus SocketStatus { get; private set; }
        public TimeSpan Timeout = TimeSpan.FromSeconds(1);

        public bool IsConnected => !((_socket.Poll(1000, SelectMode.SelectRead) && (_socket.Available == 0)) || !_socket.Connected);
        public EndPoint RemoteEndPoint => _socket.RemoteEndPoint;

        public RedClient(string targetHost = null, bool checkCertificateRevocation = true) {
            _connectDone = new ManualResetEventAsync(false);
            _targetHost = targetHost;
            _checkCertificateRevocation = checkCertificateRevocation;
            SocketStatus = SocketStatus.Disconnected;
        }

        public RedClient(X509Certificate certificate, string targetHost = null, bool checkCertificateRevocation = true) {
            _certificate = certificate;
            _targetHost = targetHost;
            _connectDone = new ManualResetEventAsync(false);
            _checkCertificateRevocation = checkCertificateRevocation;
            SocketStatus = SocketStatus.Disconnected;
        }

        internal RedClient(System.Net.Sockets.Socket socket, X509Certificate serverCertificate, bool isEncrypted) {
            _socket = socket;
            _certificate = serverCertificate;
            _isEncrypted = isEncrypted;
            SocketStatus = SocketStatus.Initializing;
            _networkStream = new NetworkStream(_socket);
            _connectDone = new ManualResetEventAsync(false);

            if (_isEncrypted) {
                _sslStream = new SslStream(_networkStream);
                Stream = _sslStream;
            } else {
                Stream = _networkStream;
            }
        }

        #region SSL Authentication

        /*  Authentication protocol bytes
         * Client -> Server: [0] = 0x06 ACK or 0x15 NAK
         * Client <- Server: [0] = 0x06 ACK or 0x15 NAK
         */

        private async Task AuthenticateAsClient() {
            try {
                if (_certificate != null) {
                    var certificateCollection = new X509CertificateCollection { _certificate };
                    await _sslStream.AuthenticateAsClientAsync(_targetHost, certificateCollection, _checkCertificateRevocation);
                } else {
                    await _sslStream.AuthenticateAsClientAsync(_targetHost);
                }

                _sslStream.WriteByte(0x06);
                int ack = _sslStream.ReadByte();
                if (ack != 0x06) throw new Exception("The server did not accept the client.");
                SocketStatus = SocketStatus.Connected;
            } catch (Exception) {
                SocketStatus = SocketStatus.Disconnected;
                _socket.Disconnect(false);
                await _sslStream.DisposeAsync();
                await _networkStream.DisposeAsync();
                _socket.Dispose();
            }
        }

        private async Task AuthenticateAsServer() {
            try {
                await _sslStream.AuthenticateAsServerAsync(_certificate);
                int ack = _sslStream.ReadByte();
                if (ack != 0x06) throw new Exception("Unknown client error.");
                _sslStream.WriteByte(0x06);
                SocketStatus = SocketStatus.Connected;
            } catch (Exception) {
                SocketStatus = SocketStatus.Disconnected;
                _socket.Disconnect(false);
                await _sslStream.DisposeAsync();
                await _networkStream.DisposeAsync();
                _socket.Dispose();
            }
        }

        #endregion

        #region Initialize Socket

        /*  Init protocol bytes
         * Client -> Server: [0-8] = [0x52, 0x45, 0x44, 0x53, 0x4f, 0x43, 0x4b, 0x45, 0x54] -> "REDSOCKET"
         * Client -> Server: [9] = Major version(0-255)
         * Client -> Server: [10] = Minor version(0-255)
         * Client -> Server: [11] = Patch version(0-255)
         *
         * Client <- Server: [0] = 0x06 ACK or 0x15 NAK
         * Client <- Server: [1] = Bool options by bit. { [0] = NoDelay, [1] = UseEncryption, [2, 3, 4, 5, 6, 7] = Unused }
         *
         * Client -> Server: [0] = 0x06 ACK or 0x15 NAK
         *
         * Client <- Server: [0] = 0x04 Handshake ends, starts encryption if necessary.
         */

        private async Task InitializeSocketAsClient() {
            SocketStatus = SocketStatus.Initializing;
            var buffer = new byte[] { 0x52, 0x45, 0x44, 0x53, 0x4f, 0x43, 0x4b, 0x45, 0x54, SocketVersion.Major, SocketVersion.Minor, SocketVersion.Patch };
            await _networkStream.WriteAsync(buffer, 0, buffer.Length);

            buffer[0] = 0;
            buffer[1] = 0;

            int bytesRead = await _networkStream.ReadAsync(buffer, 0, 2);
            if (bytesRead != 2) throw new Exception("Handshake error, protocol was not followed correctly.");
            if (buffer[0] != 0x06) throw new Exception("Handshake error, the server did not accept the socket.");
            _socket.NoDelay = GetBitAddress(buffer[1], 0);
            _isEncrypted = GetBitAddress(buffer[1], 1);

            buffer[0] = 0x06;
            await _networkStream.WriteAsync(buffer, 0, 1);

            bytesRead = await _networkStream.ReadAsync(buffer, 0, 1);
            if (bytesRead != 1) throw new Exception("Handshake error, protocol was not followed correctly.");
            if (buffer[0] == 0x04) {
                if (_isEncrypted) {
                    SocketStatus = SocketStatus.Authenticating;
                    _sslStream = new SslStream(_networkStream);
                    await AuthenticateAsClient();
                } else {
                    Stream = _networkStream;
                    SocketStatus = SocketStatus.Connected;
                }
            } else
                throw new Exception("Handshake error, protocol was not followed correctly.");
        }

        internal async Task InitializeSocketAsServer() {
            SocketStatus = SocketStatus.Initializing;
            var buffer = new byte[9];
            int bytesRead = await _networkStream.ReadAsync(buffer, 0, 9);
            if (bytesRead != 9) throw new Exception("Handshake error, protocol was not followed correctly.");
            var header = new byte[] { 0x52, 0x45, 0x44, 0x53, 0x4f, 0x43, 0x4b, 0x45, 0x54 };
            if (!buffer.SequenceEqual(header)) throw new Exception("Handshake error, protocol was not followed correctly.");
            bytesRead = await _networkStream.ReadAsync(buffer, 0, 3);
            if (bytesRead != 3) throw new Exception("Handshake error, protocol was not followed correctly.");
            if (!buffer[0].Equals(SocketVersion.Major) || !buffer[1].Equals(SocketVersion.Minor)) {
                buffer[0] = 0x15;
                buffer[1] = 0x15;
                await _networkStream.WriteAsync(buffer, 0, 2);
                await DisconnectAsync();
                //throw new Exception("Handshake error, client uses a newer or older version of the protocol.");
            }

            buffer[0] = 0x06;
            buffer[1] = 0;
            buffer[1] = SetBitAddress(buffer[1], 0, true);
            buffer[1] = SetBitAddress(buffer[1], 1, _isEncrypted);
            await _networkStream.WriteAsync(buffer, 0, 2);

            bytesRead = await _networkStream.ReadAsync(buffer, 0, 1);
            if (bytesRead != 1) throw new Exception("Handshake error, protocol was not followed correctly.");
            if (buffer[0] != 0x06) {
                await DisconnectAsync();
            }

            buffer[0] = 0x04;
            await _networkStream.WriteAsync(buffer, 0, 1);
            if (_isEncrypted) {
                SocketStatus = SocketStatus.Authenticating;
                _sslStream = new SslStream(_networkStream);
                await AuthenticateAsServer();
            } else {
                Stream = _networkStream;
                SocketStatus = SocketStatus.Connected;
            }
        }

        #endregion

        public async Task ConnectAsync(IPEndPoint endPoint, string targetHost = null) {
            if (SocketStatus != SocketStatus.Disconnected) throw new SocketException(10056);
            _targetHost ??= targetHost;
            _socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.BeginConnect(endPoint, BeginConnectAsyncResult, null);
            if (!await _connectDone.WaitAsync(Timeout))
                if (_connectException != null)
                    throw _connectException ?? new SocketException(10057);
            _connectDone.Reset();
            if (_connectException != null) throw _connectException;
            if (_socket.Connected) {
                try {
                    _socket.NoDelay = true;
                    _networkStream = new NetworkStream(_socket);
                    var initTask = InitializeSocketAsClient();
                    if (await Task.WhenAny(initTask, Task.Delay(Timeout)) != initTask) {
                        await DisconnectAsync();
                        throw new Exception("Timeout reached.");
                    }
                    await initTask;
                } catch (Exception) {
                    SocketStatus = SocketStatus.Disconnected;
                    await DisconnectAsync();
                    throw;
                }
            } else {
                SocketStatus = SocketStatus.Disconnected;
                throw new SocketException(10057);
            }
        }

        private void BeginConnectAsyncResult(IAsyncResult ar) {
            _connectException = null;
            try {
                _socket.EndConnect(ar);
            } catch (Exception ex) {
                _connectException = ex;
            }

            _connectDone.Set();
        }

        public async Task DisconnectAsync() {
            SocketStatus = SocketStatus.Disconnected;
            if (_socket == null || !_socket.Connected) return;
            if (_sslStream != null) await _sslStream.DisposeAsync();
            if (_networkStream != null) await _networkStream.DisposeAsync();
            if (_socket != null) {
                _socket.Disconnect(false);
                _socket.Dispose();
            }
            SocketStatus = SocketStatus.Disconnected;
        }

        private static bool GetBitAddress(byte byteToConvert, int bitToReturn) {
            int mask = 1 << bitToReturn;
            return (byteToConvert & mask) == mask;
        }

        private static byte SetBitAddress(byte byteToConvert, int bitToSet, bool value) {
            int mask = 1 << bitToSet;
            if (value) {
                byteToConvert |= (byte)mask;
            } else {
                byteToConvert &= (byte)~mask;
            }

            return byteToConvert;
        }

        public void Dispose() {
            Stream?.Dispose();
            _networkStream?.Dispose();
            _sslStream?.Dispose();
            _socket?.Dispose();
        }
    }
}