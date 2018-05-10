using System;
using System.Diagnostics;
using System.Text;
using Medella.TdsClient.SNI.Native.Sspi;

namespace Medella.TdsClient.SNI.Native
{
    public class SniNative : ISniHandle
    {
        private readonly Guid _clientConnectionId;
        private readonly SNIHandle _sniHandle;
        private readonly SspiNative _sspi;
        private SniPacket _writePacket;
        public byte[] InstanceNameBytes;

        public SniNative(string serverName, int timeout)
        {
            var status = SniLoadHandle.SingletonInstance.SniStatus;
            _sniHandle = new SNIHandle(".", timeout, out InstanceNameBytes);
            ServerSpn = Encoding.ASCII.GetString(_sniHandle.SpnBuffer);
            InstanceName = "";
            _sspi = new SspiNative(_sniHandle, _sniHandle.SpnBuffer);
            SniNativeMethodWrapper.SniGetConnectionId(_sniHandle, ref _clientConnectionId);
        }

        public string ServerSpn { get; }
        public string InstanceName { get; }

        public void FlushBuffer(byte[] writeBuffer, int count)
        {
            GetBytesString("Write-", writeBuffer, count);
            if (_writePacket != null)
                SniNativeMethodWrapper.SNIPacketReset(_sniHandle, SniNativeMethodWrapper.IOType.WRITE, _writePacket, SniNativeMethodWrapper.ConsumerNumber.SNI_Consumer_SNI);
            else
                _writePacket = new SniPacket(_sniHandle);
            SniNativeMethodWrapper.SNIPacketSetData(_writePacket, writeBuffer, count);
            SniNativeMethodWrapper.SNIWritePacket(_sniHandle, _writePacket, true);
        }

        public int Receive(byte[] readBuffer, int offset, int count)
        {
            var readPacketPtr = IntPtr.Zero;
            var error = SniNativeMethodWrapper.SNIReadSyncOverAsync(_sniHandle, ref readPacketPtr, 15000);
            var dataSize = (uint) count;
            SniNativeMethodWrapper.SNIPacketGetData(readPacketPtr, readBuffer, ref dataSize);
            GetBytesString("Read- ", readBuffer, (int) dataSize);
            SniNativeMethodWrapper.SNIPacketRelease(readPacketPtr);
            return (int) dataSize;
        }

        public byte[] GetClientToken(byte[] serverToken)
        {
            return _sspi.GetClientToken(serverToken);
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

        public void Dispose()
        {
            _sniHandle?.Dispose();
            _writePacket?.Dispose();
            _sspi?.Dispose();
        }
    }
}