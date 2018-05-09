using System;
using System.Runtime.InteropServices;
using Medella.TdsClient.SNI.Native;

namespace Medella.TdsClient.SNI.Internal
{
    internal sealed class SniHandle : SafeHandle
    {
        private readonly bool _fSync;

        public SniHandle(IntPtr invalidHandleValue, bool ownsHandle) : base(invalidHandleValue, ownsHandle)
        {
        }

        // creates a physical connection


        public override bool IsInvalid => IntPtr.Zero == handle;


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