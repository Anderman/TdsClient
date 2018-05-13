using System.Diagnostics;
using System.IO.Pipes;
using System.Net;
using System.Net.Security;
using System.Text;
using Medella.TdsClient.SNI.Sspi;

namespace Medella.TdsClient.SNI.SniNp
{
    public class SniNpHandle : ISniHandle
    {
        private readonly SslOverTdsStream _sslOverTdsStream;
        private SslStream _sslStream;
        private readonly SspiHelper _sspi;
        public NamedPipeClientStream Stream;


        public SniNpHandle(string serverName, string pipeName, long timeOut)
        {
            Stream = new NamedPipeClientStream(serverName, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);

            if (timeOut >= int.MaxValue)
                Stream.Connect();
            else
                Stream.Connect((int) timeOut);

            _sspi = new SspiHelper(ServerSpn);
        }

        public string ServerSpn => $"MSSQLSvc/{Dns.GetHostEntry("localhost").HostName}"; //needed for sspi login
        public string InstanceName { get; } = ""; //needed when connect 

        public int Receive(byte[] readBuffer, int offset, int count)
        {
            var len = Stream.Read(readBuffer, offset, count);
            GetBytesString("Read- ", readBuffer, len);
            return len;
        }

        public void FlushBuffer(byte[] writeBuffer, int count)
        {
            GetBytesString("Write-", writeBuffer, count);
            //Stream.WriteAsync(writeBuffer, 0, count);
            Stream.Write(writeBuffer, 0, count);
        }

        public byte[] GetClientToken(byte[] serverToken)
        {
            _sspi.CreateClientToken(serverToken);
            return _sspi.ClientToken;
        }

        public void Dispose()
        {
            _sslOverTdsStream?.Dispose();
            _sslStream?.Dispose();
            Stream?.Dispose();
        }

        [Conditional("DEBUG")]
        private static void GetBytesString(string prefix, byte[] buffer, int length)
        {
            var sb = new StringBuilder($"{prefix}lentgh:{length,4:##0} ");
            sb.Append("data: ");
            for (var i = 0; i < length; i++)
                sb.Append($"{buffer[i],2:X2} ");
            Debug.WriteLine(sb.ToString());
            sb = new StringBuilder($"{prefix}lentgh:{length,4:##0} ");
            sb.Append("data: ");
            for (var i = 0; i < length; i++)
                if (buffer[i] >= 0x20 && buffer[i] <= 0x7f)
                    sb.Append($"{(char) buffer[i]}");
            Debug.WriteLine(sb.ToString());
        }
    }
}