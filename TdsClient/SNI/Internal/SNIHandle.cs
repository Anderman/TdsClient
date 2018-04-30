using System;
using System.Runtime.InteropServices;
using System.Threading;
using Medella.TdsClient.Contants;

namespace Medella.TdsClient.SNI.Internal
{
    internal sealed class SniHandle : SafeHandle
    {
        private readonly bool _fSync;

        // creates a physical connection
        internal SniHandle(
            SniNativeMethodWrapper.ConsumerInfo myInfo,
            string serverName,
            byte[] spnBuffer,
            bool ignoreSniOpenTimeout,
            int timeout,
            out byte[] instanceName,
            bool flushCache,
            bool fSync,
            bool fParallel)
            : base(IntPtr.Zero, true)
        {
            try
            {
            }
            finally
            {
                _fSync = fSync;
                instanceName = new byte[256]; // Size as specified by netlibs.
                if (ignoreSniOpenTimeout) timeout = Timeout.Infinite; // -1 == native SNIOPEN_TIMEOUT_VALUE / INFINITE

                Status = SniNativeMethodWrapper.SNIOpenSyncEx(myInfo, serverName, ref handle,
                    spnBuffer, instanceName, flushCache, fSync, timeout, fParallel);
            }
        }

        // constructs SNI Handle for MARS session
        internal SniHandle(SniNativeMethodWrapper.ConsumerInfo myInfo, SniHandle parent) : base(IntPtr.Zero, true)
        {
            try
            {
            }
            finally
            {
                Status = SniNativeMethodWrapper.SNIOpenMarsSession(myInfo, parent, ref handle, parent._fSync);
            }
        }

        public override bool IsInvalid => IntPtr.Zero == handle;

        internal uint Status { get; } = TdsEnums.SNI_UNINITIALIZED;

        protected override bool ReleaseHandle()
        {
            // NOTE: The SafeHandle class guarantees this will be called exactly once.
            var ptr = handle;
            handle = IntPtr.Zero;
            if (IntPtr.Zero == ptr)
                return true;
            return SniNativeMethodWrapper.SNIClose(ptr) == 0;
        }
    }
}