using System;
using System.Runtime.InteropServices;

namespace Medella.TdsClient.System2.Net.Security
{
    [StructLayout(LayoutKind.Sequential)]
    internal class SecPkgContext_ConnectionInfo
    {
        public readonly int Protocol;
        public readonly int DataCipherAlg;
        public readonly int DataKeySize;
        public readonly int DataHashAlg;
        public readonly int DataHashKeySize;
        public readonly int KeyExchangeAlg;
        public readonly int KeyExchKeySize;

        internal unsafe SecPkgContext_ConnectionInfo(byte[] nativeBuffer)
        {
            fixed (void* voidPtr = nativeBuffer)
            {
                try
                {
                    // TODO (Issue #3114): replace with Marshal.PtrToStructure.
                    IntPtr unmanagedAddress = new IntPtr(voidPtr);
                    Protocol = Marshal.ReadInt32(unmanagedAddress);
                    DataCipherAlg = Marshal.ReadInt32(unmanagedAddress, 4);
                    DataKeySize = Marshal.ReadInt32(unmanagedAddress, 8);
                    DataHashAlg = Marshal.ReadInt32(unmanagedAddress, 12);
                    DataHashKeySize = Marshal.ReadInt32(unmanagedAddress, 16);
                    KeyExchangeAlg = Marshal.ReadInt32(unmanagedAddress, 20);
                    KeyExchKeySize = Marshal.ReadInt32(unmanagedAddress, 24);
                }
                catch (OverflowException)
                {
                    NetEventSource.Fail(this, "Negative size");
                    throw;
                }
            }
        }
    }
}