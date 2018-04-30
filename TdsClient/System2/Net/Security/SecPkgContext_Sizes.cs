using System;
using System.Runtime.InteropServices;

namespace Medella.TdsClient.System2.Net.Security
{
    [StructLayout(LayoutKind.Sequential)]
    internal class SecPkgContext_Sizes
    {
        public readonly int cbMaxToken;
        public readonly int cbMaxSignature;
        public readonly int cbBlockSize;
        public readonly int cbSecurityTrailer;

        internal unsafe SecPkgContext_Sizes(byte[] memory)
        {
            fixed (void* voidPtr = memory)
            {
                IntPtr unmanagedAddress = new IntPtr(voidPtr);
                try
                {
                    // TODO (Issue #3114): replace with Marshal.PtrToStructure.
                    cbMaxToken = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress));
                    cbMaxSignature = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 4));
                    cbBlockSize = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 8));
                    cbSecurityTrailer = (int)checked((uint)Marshal.ReadInt32(unmanagedAddress, 12));
                }
                catch (OverflowException)
                {
                    NetEventSource.Fail(this, "Negative size.");
                    throw;
                }
            }
        }

        public static readonly int SizeOf = Marshal.SizeOf<SecPkgContext_Sizes>();
    }
}