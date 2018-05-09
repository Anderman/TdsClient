using System;
using System.Runtime.InteropServices;
using Medella.TdsClient.Contants;

namespace Medella.TdsClient.SNI.Native
{
    public class SNIHandle : SafeHandle
    {
        private readonly bool _fSync;

        // creates a physical connection
        public SNIHandle(string serverName, int timeout, out byte[] instanceName) : base(IntPtr.Zero, true)
        {
            var myInfo = new SniNativeMethodWrapper.ConsumerInfo {defaultBufferSize = 8000};
            SpnBuffer = new byte[SniNativeMethodWrapper.SniMaxComposedSpnLength];
            timeout = timeout * 1000;
            _fSync = false;
            instanceName = new byte[256]; // Size as specified by netlibs.

            Status = SniNativeMethodWrapper.SNIOpenSyncEx(myInfo, serverName, ref handle, SpnBuffer, instanceName, false, false, timeout, false);
        }

        public override bool IsInvalid => IntPtr.Zero == handle;
        public uint Status { get; } = TdsEnums.SNI_UNINITIALIZED;
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