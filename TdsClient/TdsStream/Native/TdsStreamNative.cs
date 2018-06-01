using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TdsStream.Sspi;

namespace Medella.TdsClient.TdsStream.Native
{
    public class TdsStreamNative : ITdsStream
    {
        private readonly Guid _clientConnectionId;
        private readonly SniNativeHandle _sniNativeHandle;
        private readonly SspiNative _sspi;
        private SniPacket _writePacket;
        public byte[] InstanceNameBytes;

        public TdsStreamNative(string serverName, int timeout)
        {
            var status = SniLoadHandle.SingletonInstance.SniStatus;
            _sniNativeHandle = new SniNativeHandle(serverName, timeout, out InstanceNameBytes);
            ServerSpn = Encoding.ASCII.GetString(_sniNativeHandle.SpnBuffer);
            InstanceName = "";
            _sspi = new SspiNative(_sniNativeHandle, _sniNativeHandle.SpnBuffer);
            SniNativeMethodWrapper.SniGetConnectionId(_sniNativeHandle, ref _clientConnectionId);
        }

        public string ServerSpn { get; }
        public string InstanceName { get; }

        public void FlushBuffer(byte[] writeBuffer, int count)
        {
            GetBytesString("Write-", writeBuffer, count);
            if (_writePacket != null)
                SniNativeMethodWrapper.SNIPacketReset(_sniNativeHandle, SniNativeMethodWrapper.IOType.WRITE, _writePacket, SniNativeMethodWrapper.ConsumerNumber.SNI_Consumer_SNI);
            else
                _writePacket = new SniPacket(_sniNativeHandle);
            SniNativeMethodWrapper.SNIPacketSetData(_writePacket, writeBuffer, count);
            SniNativeMethodWrapper.SNIWritePacket(_sniNativeHandle, _writePacket, true);
        }

        public async Task<int> ReceiveAsync(byte[] readBuffer, int offset, int count)
        {
            return await Task.Run(() => Receive(readBuffer, offset, count));
        }
        public int Receive(byte[] readBuffer, int offset, int count)
        {
            var readPacketPtr = IntPtr.Zero;
            var error = SniNativeMethodWrapper.SNIReadSyncOverAsync(_sniNativeHandle, ref readPacketPtr, 15000);
            if (error != TdsEnums.SNI_SUCCESS)
            {
                SniNativeMethodWrapper.SNIGetLastError(out var errorStruct);
                throw new Exception(errorStruct.errorMessage);
            }
            var dataSize = (uint)count;
            SniNativeMethodWrapper.SNIPacketGetData(readPacketPtr, readBuffer, ref dataSize);
            GetBytesString("Read- ", readBuffer, (int)dataSize);
            SniNativeMethodWrapper.SNIPacketRelease(readPacketPtr);
            return (int)dataSize;
        }

        public byte[] GetClientToken(byte[] serverToken)
        {
            return _sspi.GetClientToken(serverToken);
        }

        public void Dispose()
        {
            _sniNativeHandle?.Dispose();
            _writePacket?.Dispose();
            _sspi?.Dispose();
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
    }
}