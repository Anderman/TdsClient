using System;
using System.Runtime.InteropServices;
using Medella.TdsClient.Exceptions;

namespace Medella.TdsClient.SNI.Native
{
    public sealed class SniPacket : SafeHandle
    {
        public override bool IsInvalid => (IntPtr.Zero == handle);

        public SniPacket(SafeHandle sniHandle) : base(IntPtr.Zero, true)
        {
            SniNativeMethodWrapper.SNIPacketAllocate(sniHandle, SniNativeMethodWrapper.IOType.WRITE, ref handle);
            if (IntPtr.Zero == handle)
                throw SQL.SNIPacketAllocationFailure();
        }

        protected override bool ReleaseHandle()
        {
            // NOTE: The SafeHandle class guarantees this will be called exactly once.
            var ptr = base.handle;
            handle = IntPtr.Zero;
            if (IntPtr.Zero != ptr)
                SniNativeMethodWrapper.SNIPacketRelease(ptr);
            return true;
        }
    }
}