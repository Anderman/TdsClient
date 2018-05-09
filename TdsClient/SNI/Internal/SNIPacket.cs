using System;
using System.Runtime.InteropServices;
using Medella.TdsClient.SNI.Native;

namespace Medella.TdsClient.SNI.Internal
{
    public sealed class SniPacket : SafeHandle
    {
        public SniPacket(IntPtr invalidHandleValue, bool ownsHandle) : base(invalidHandleValue, ownsHandle)
        {
        }

        public override bool IsInvalid => IntPtr.Zero == handle;

        protected override bool ReleaseHandle()
        {
            // NOTE: The SafeHandle class guarantees this will be called exactly once.
            var ptr = handle;
            handle = IntPtr.Zero;
            if (IntPtr.Zero != ptr) SniNativeMethodWrapper.SNIPacketRelease(ptr);
            return true;
        }
    }
}