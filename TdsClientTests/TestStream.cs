using System;
using System.Collections.Generic;
using Medella.TdsClient.TdsStream;

namespace TdsClientTests
{
    public class TestStream : ITdsStream
    {
        public Queue<byte[]> Queue = new Queue<byte[]>();
        public string ServerSpn { get; }
        public string InstanceName { get; }

        public void FlushBuffer(byte[] writeBuffer, int count)
        {
            var package = new byte[count];
            Buffer.BlockCopy(writeBuffer, 0, package, 0, count);
            Queue.Enqueue(package);
        }

        public int Receive(byte[] readBuffer, int offset, int count)
        {
            var package = Queue.Dequeue();
            Buffer.BlockCopy(package, 0, readBuffer, 0, package.Length);
            return package.Length;
        }

        public byte[] GetClientToken(byte[] serverToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}