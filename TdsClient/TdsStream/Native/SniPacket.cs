using System;
using System.Runtime.InteropServices;
using Medella.TdsClient.Exceptions;

namespace Medella.TdsClient.TdsStream.Native
{
    public sealed class SniPacket : SafeHandle
    {
        public SniPacket(SafeHandle sniHandle) : base(IntPtr.Zero, true)
        {
            SniNativeMethodWrapper.SNIPacketAllocate(sniHandle, SniNativeMethodWrapper.IOType.WRITE, ref handle);
            if (IntPtr.Zero == handle)
                throw SQL.SniPacketAllocationFailure();
        }

        public override bool IsInvalid => IntPtr.Zero == handle;

        protected override bool ReleaseHandle()
        {
            // NOTE: The SafeHandle class guarantees this will be called exactly once.
            var ptr = handle;
            handle = IntPtr.Zero;
            if (IntPtr.Zero != ptr)
                SniNativeMethodWrapper.SNIPacketRelease(ptr);
            return true;
        }
    }
}