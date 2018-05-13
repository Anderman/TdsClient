using System;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.TdsStream.Native;

namespace Medella.TdsClient.TdsStream.Sspi
{
    public class SspiNative : IDisposable
    {
        private static volatile uint _sMaxSspiLength;
        private readonly SniNativeHandle _sniNativeHandle;
        private readonly byte[] _sniSpnBuffer;

        static SspiNative()
        {
            // use local for ref param to defer setting s_maxSSPILength until we know the call succeeded.
            uint maxLength = 0;

            if (0 != SniNativeMethodWrapper.SNISecInitPackage(ref maxLength))
                throw new Exception("Cannot load sspi");

            _sMaxSspiLength = maxLength;

            if (_sMaxSspiLength > int.MaxValue)
                throw SQL.InvalidSSPIPacketSize(); // SqlBu 332503
        }

        public SspiNative(SniNativeHandle sniNativeHandle, byte[] sniSpnBuffer)
        {
            _sniNativeHandle = sniNativeHandle;
            _sniSpnBuffer = sniSpnBuffer;
        }

        public void Dispose()
        {
            _sniNativeHandle?.Dispose();
        }

        public byte[] GetClientToken(byte[] serverToken)
        {
            var clientToken = new byte[_sMaxSspiLength];
            var length = (uint) clientToken.Length;
            SniSspiData(serverToken, (uint) (serverToken?.Length ?? 0), ref clientToken, ref length);
            if (length > int.MaxValue)
                throw SQL.InvalidSSPIPacketSize(); // SqlBu 332503
            Array.Resize(ref clientToken, (int) length);
            return clientToken;
        }

        internal void SniSspiData(byte[] receivedBuff, uint receivedLength, ref byte[] sendBuff, ref uint sendLength)
        {
            if (receivedBuff == null) receivedLength = 0;

            // we need to respond to the server's message with SSPI data
            if (SniSecGenClientContext(receivedBuff, receivedLength, sendBuff, ref sendLength) != 0)
                throw new Exception("Failed to get sspi clientcontext");
        }

        internal unsafe uint SniSecGenClientContext(byte[] inBuff, uint receivedLength, byte[] outBuff, ref uint sendLength)
        {
            fixed (byte* pinServerUserName = &_sniSpnBuffer[0])
            {
                return SniNativeMethodWrapper.SNISecGenClientContextWrapper(_sniNativeHandle, inBuff, receivedLength, outBuff, ref sendLength, out _, pinServerUserName, (uint) _sniSpnBuffer.Length, null, null);
            }
        }
    }
}