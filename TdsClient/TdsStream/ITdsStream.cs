using System;
using System.Threading.Tasks;

namespace Medella.TdsClient.TdsStream
{
    public interface ITdsStream : IDisposable
    {
        string InstanceName { get; }
        void FlushBuffer(byte[] writeBuffer, int count);
        int Receive(byte[] readBuffer, int offset, int count);
        Task<int> ReceiveAsync(byte[] readBuffer, int offset, int count);

        byte[] GetClientToken(byte[]? serverToken);
    }
}