using System;
using System.Runtime.InteropServices;

namespace Medella.TdsClient.TdsStream.Native
{
    public class SniNativeHandle : SafeHandle
    {
        // creates a physical connection
        public SniNativeHandle(string serverName, int timeoutSec, out byte[] instanceName) : base(IntPtr.Zero, true)
        {
            var myInfo = new SniNativeMethodWrapper.ConsumerInfo {defaultBufferSize = 8000};
            SpnBuffer = new byte[SniNativeMethodWrapper.SniMaxComposedSpnLength];
            var timeoutmSec = timeoutSec * 1000;
            instanceName = new byte[256]; // Size as specified by netlibs.

            Status = SniNativeMethodWrapper.SNIOpenSyncEx(myInfo, serverName, ref handle, SpnBuffer, instanceName, false, false, timeoutmSec, false);
        }

        public override bool IsInvalid => IntPtr.Zero == handle;
        public uint Status { get; }
        public byte[] SpnBuffer { get; set; }

        protected override bool ReleaseHandle()
        {
            // NOTE: The SafeHandle class guarantees this will be called exactly once.
            var ptr = handle;
            handle = IntPtr.Zero;
            if (IntPtr.Zero == ptr)
                return true;
            return 0 == SniNativeMethodWrapper.SNIClose(ptr);
        }
    }
}