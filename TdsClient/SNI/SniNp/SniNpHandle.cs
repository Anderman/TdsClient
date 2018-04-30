using System.Diagnostics;
using System.IO.Pipes;
using System.Net.Security;
using System.Text;

namespace Medella.TdsClient.SNI.SniNp
{
    public class SniNpHandle
    {
        private readonly SslOverTdsStream _sslOverTdsStream;
        private SslStream _sslStream;
        public NamedPipeClientStream Stream;
        private int _lastcount;
        private int _lastWriteCount;
        private int _lastReadCount;


        public SniNpHandle(string serverName, string pipeName, long timeOut)
        {
            var pipeStream = new NamedPipeClientStream(serverName, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);

            if (timeOut >= int.MaxValue)
                pipeStream.Connect();
            else
                pipeStream.Connect((int)timeOut);

            Stream = pipeStream;
        }

        public int Receive(byte[] readBuffer, int offset, int count)
        {
            if (_lastcount == _lastWriteCount)
            {
                GetBytesString("Read- ", readBuffer, _lastReadCount);
                return _lastReadCount;
            }
            _lastcount = _lastWriteCount;
            var len = Stream.Read(readBuffer, offset, count);
            _lastReadCount = len;
            GetBytesString("Read- ", readBuffer, len);
            return len;
        }

        internal void FlushBuffer(byte[] writeBuffer, int count)
        {
            GetBytesString("Write-", writeBuffer, count);
            //Stream.WriteAsync(writeBuffer, 0, count);
            if (count != _lastWriteCount)
                Stream.Write(writeBuffer, 0, count);
            _lastWriteCount = count;
        }

        [Conditional("DEBUG")]
        private static void GetBytesString(string prefix, byte[] buffer, int length)
        {
            var sb = new StringBuilder($"{prefix}lentgh:{length,4:##0} ");
            sb.Append("data: ");
            for (var i = 0; i < length; i++)
                sb.Append($"{buffer[i],2:X2} ");
            Debug.WriteLine(sb.ToString());
        }

        //private void CheckBuffer(int size)
        //{
        //    var left = _endPos - _readEndPos;
        //    if (size <= left)
        //        return;
        //    if (size > BufferSize)
        //    {
        //        ResultInLargeBuffer(size, left);
        //    }
        //    else
        //    {
        //        Buffer.BlockCopy(ReadBuffer, _readEndPos, ReadBuffer, 0, left);
        //        _endPos = Stream.Read(ReadBuffer, left, BufferSize - left) + left;
        //        _readEndPos = 0;
        //    }
        //}
    }
}