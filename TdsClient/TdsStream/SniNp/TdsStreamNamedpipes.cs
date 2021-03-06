﻿using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Medella.TdsClient.TdsStream.Sspi;

namespace Medella.TdsClient.TdsStream.SniNp
{
    public class TdsStreamNamedPipes : ITdsStream
    {
        private readonly SslOverTdsStream? _sslOverTdsStream;
        private readonly SspiHelper _sspi;
        private SslStream? _sslStream;
        public NamedPipeClientStream Stream;


        public TdsStreamNamedPipes(string serverName, string pipeName, long timeOut)
        {
            Stream = new NamedPipeClientStream(serverName, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
            timeOut = timeOut * 1000;
            if (timeOut >= int.MaxValue)
                Stream.Connect();
            else
                Stream.Connect((int)timeOut);

            _sspi = new SspiHelper(ServerSpn);
        }

        public string ServerSpn => $"MSSQLSvc/{Dns.GetHostEntry("localhost").HostName}"; //needed for sspi login
        public string InstanceName { get; } = ""; //needed when connect 

        public int Receive(byte[] readBuffer, int offset, int count)
        {
            if (!Stream.IsConnected)
                throw new Exception("Not connected");
            var len = Stream.Read(readBuffer, offset, count);
            if (!Stream.IsConnected)
                throw new Exception("Not connected");
            GetBytesString("Read- ", readBuffer, len);
            return len;
        }

        public async Task<int> ReceiveAsync(byte[] readBuffer, int offset, int count)
        {
            if (!Stream.IsConnected)
                throw new Exception("Not connected");
            var len = await Stream.ReadAsync(readBuffer, offset, count);
            if (!Stream.IsConnected)
                throw new Exception("Not connected");
            GetBytesString("Read- ", readBuffer, len);
            return len;
        }

        public void FlushBuffer(byte[] writeBuffer, int count)
        {
            GetBytesString("Write-", writeBuffer, count);
            //Stream.WriteAsync(writeBuffer, 0, count);
            Stream.Write(writeBuffer, 0, count);
        }

        public byte[] GetClientToken(byte[]? serverToken)
        {
            return _sspi.CreateClientToken(serverToken);
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
            var sb = new StringBuilder($"{prefix}length:{length,4:##0} ");
            sb.Append("data: ");
            for (var i = 0; i < length; i++)
                sb.Append($"{buffer[i],2:X2} ");
            Debug.WriteLine(sb.ToString());
            sb = new StringBuilder($"{prefix}length:{length,4:##0} ");
            sb.Append("data: ");
            for (var i = 0; i < length; i++)
                if (buffer[i] >= 0x20 && buffer[i] <= 0x7f)
                    sb.Append($"{(char)buffer[i]}");
            Debug.WriteLine(sb.ToString());
        }
    }
}