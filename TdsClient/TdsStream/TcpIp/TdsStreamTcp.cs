using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TdsStream.Sspi;

namespace Medella.TdsClient.TdsStream.TcpIp
{
    /// <summary>
    ///     TCP connection handle
    /// </summary>
    internal class TdsStreamTcp : ITdsStream
    {
        private const int DefaultSqlServerPort = 1433;
        private readonly SspiHelper _sspi;
        private readonly string _targetServer;
        private Socket _socket;
        private SslOverTdsStream _sslOverTdsStream;
        private SslStream _sslStream;
        private Stream _stream;
        private NetworkStream _tcpStream;
        private bool _validateCert = true;

        public TdsStreamTcp(string serverName, int port, int timeoutSec)
        {
            _targetServer = serverName;

            var ts = new TimeSpan(0, 0, timeoutSec);

            _socket = Connect(serverName, port == -1 ? DefaultSqlServerPort : port, ts);

            if (_socket == null || !_socket.Connected)
                throw new Exception($"Connection Failed to server'{serverName}:{port}'");

            _socket.NoDelay = true;
            _tcpStream = new NetworkStream(_socket, true);

            _sslOverTdsStream = new SslOverTdsStream(_tcpStream);
            _sslStream = new SslStream(_sslOverTdsStream, true, ValidateServerCertificate, null);
            _stream = _tcpStream;
            var serverSpn = GetSqlServerSpn(serverName, port);
            _sspi = new SspiHelper(serverSpn);
        }

        public void Dispose()
        {
            _sslOverTdsStream?.Dispose();
            _sslOverTdsStream = null;
            _sslStream?.Dispose();
            _sslStream = null;
            _tcpStream?.Dispose();
            _tcpStream = null;

            //Release any references held by _stream.
            _stream = null;
            _socket?.Dispose();
            _socket = null;
        }

        public string InstanceName { get; } = "";

        public void FlushBuffer(byte[] writeBuffer, int count)
        {
            GetBytesString("Write-", writeBuffer, count);
            //Stream.WriteAsync(writeBuffer, 0, count);
            _stream.Write(writeBuffer, 0, count);
        }

        public async Task<int> ReceiveAsync(byte[] readBuffer, int offset, int count)
        {

            var len = await _stream.ReadAsync(readBuffer, offset, count);
            GetBytesString("Read- ", readBuffer, len);
            return len;
        }
        public int Receive(byte[] readBuffer, int offset, int count)
        {
            var len = _stream.Read(readBuffer, offset, count);
            GetBytesString("Read- ", readBuffer, len);
            return len;
        }
        [Conditional("DEBUG")]
        private static void GetBytesString(string prefix, byte[] buffer, int length)
        {
            //var sb = new StringBuilder($"{prefix}lentgh:{length,4:##0} ");
            //sb.Append("data: ");
            //for (var i = 0; i < length; i++)
            //    sb.Append($"{buffer[i],2:X2} ");
            //Debug.WriteLine(sb.ToString());
            //sb = new StringBuilder($"{prefix}lentgh:{length,4:##0} ");
            //sb.Append("data: ");
            //for (var i = 0; i < length; i++)
            //    if (buffer[i] >= 0x20 && buffer[i] <= 0x7f)
            //        sb.Append($"{(char)buffer[i]}");
            //Debug.WriteLine(sb.ToString());
        }

        public byte[] GetClientToken(byte[] serverToken)
        {
            _sspi.CreateClientToken(serverToken);
            return _sspi.ClientToken;
        }

        private string GetSqlServerSpn(string serverName, int port)
        {
            // If Server name is empty or a localhost name, then use "localhost"
            var hostName = serverName;
            var portOrInstanceName = port != -1
                ? port.ToString()
                : !string.IsNullOrWhiteSpace(InstanceName)
                    ? InstanceName
                    : DefaultSqlServerPort.ToString();

            return GetSqlServerSpn(hostName, portOrInstanceName);
        }

        private static string GetSqlServerSpn(string hostNameOrAddress, string portOrInstanceName)
        {
            var hostEntry = Dns.GetHostEntry(hostNameOrAddress);
            var fullyQualifiedDomainName = hostEntry.HostName;
            var serverSpn = $"MSSQLSvc/"+fullyQualifiedDomainName;
            if (!string.IsNullOrWhiteSpace(portOrInstanceName)) serverSpn += ":" + portOrInstanceName;
            return serverSpn;
        }

        private static Socket Connect(string serverName, int port, TimeSpan timeout)
        {
            var ipAddresses = Dns.GetHostAddresses(serverName);
            IPAddress serverIPv4 = null;
            IPAddress serverIPv6 = null;
            foreach (var ipAdress in ipAddresses)
                switch (ipAdress.AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        serverIPv4 = ipAdress;
                        break;
                    case AddressFamily.InterNetworkV6:
                        serverIPv6 = ipAdress;
                        break;
                }
            ipAddresses = new[] {serverIPv4, serverIPv6};
            var sockets = new Socket[2];

            var cts = new CancellationTokenSource();
            cts.CancelAfter(timeout);

            void Cancel()
            {
                for (var i = 0; i < sockets.Length; ++i)
                    try
                    {
                        if (sockets[i] == null || sockets[i].Connected)
                            continue;
                        sockets[i].Dispose();
                        sockets[i] = null;
                    }
                    catch
                    {
                        // ignored
                    }
            }

            cts.Token.Register(Cancel);

            Socket availableSocket = null;
            for (var i = 0; i < sockets.Length; ++i)
                try
                {
                    if (ipAddresses[i] == null)
                        continue;
                    sockets[i] = new Socket(ipAddresses[i].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    sockets[i].Connect(ipAddresses[i], port);
                    if (sockets[i] == null)
                        continue;
                    if (sockets[i].Connected)
                    {
                        availableSocket = sockets[i];
                        break;
                    }

                    sockets[i].Dispose();
                    sockets[i] = null;
                }
                catch
                {
                    // ignored
                }

            return availableSocket;
        }


        /// <summary>
        ///     Enable SSL
        /// </summary>
        public void EnableSsl(uint options)
        {
            _validateCert = (options & TdsEnums.SNI_SSL_VALIDATE_CERTIFICATE) != 0;

            _sslStream.AuthenticateAsClient(_targetServer);
            _sslOverTdsStream.FinishHandshake();

            _stream = _sslStream;
        }

        /// <summary>
        ///     Disable SSL
        /// </summary>
        public void DisableSsl()
        {
            _sslStream.Dispose();
            _sslStream = null;
            _sslOverTdsStream.Dispose();
            _sslOverTdsStream = null;

            _stream = _tcpStream;
        }

        /// <summary>
        ///     Validate server certificate callback
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="cert">X.509 certificate</param>
        /// <param name="chain">X.509 chain</param>
        /// <param name="policyErrors">Policy errors</param>
        /// <returns>True if certificate is valid</returns>
        private bool ValidateServerCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            if (!_validateCert) return true;

            return true; //SNICommon.ValidateSslServerCertificate(_targetServer, sender, cert, chain, policyErrors);
        }

        /// <summary>
        ///     Check SNI handle connection
        /// </summary>
        /// <param name="handle"></param>
        /// <returns>SNI error status</returns>
        public uint CheckConnection()
        {
            if (!_socket.Connected || _socket.Poll(0, SelectMode.SelectError)) return TdsEnums.SNI_ERROR;
            return TdsEnums.SNI_SUCCESS;
        }
    }
}