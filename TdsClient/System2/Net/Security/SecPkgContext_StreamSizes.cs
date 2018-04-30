using System;
using System.Runtime.InteropServices;

namespace Medella.TdsClient.System2.Net.Security
{
    [StructLayout(LayoutKind.Sequential)]
    internal class SecPkgContext_StreamSizes
    {
        public int cbHeader;
        public int cbTrailer;
        public int cbMaximumMessage;
        public int cBuffers;
        public int cbBlockSize;

        internal unsafe SecPkgContext_StreamSizes(byte[] memory)
        {
            fixed (void* voidPtr = memory)
            {
                var unmanagedAddress = new IntPtr(voidPtr);
                try
                {
                    // TODO (Issue #3114): replace with Marshal.PtrToStructure.
                    cbHeader = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress));
                    cbTrailer = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 4));
                    cbMaximumMessage = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 8));
                    cBuffers = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 12));
                    cbBlockSize = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 16));
                }
                catch (OverflowException)
                {
                    NetEventSource.Fail(this, "Negative size.");
                    throw;
                }
            }
        }

        public static readonly int SizeOf = Marshal.SizeOf<SecPkgContext_StreamSizes>();
    }
}