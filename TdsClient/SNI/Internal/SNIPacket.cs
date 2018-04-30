﻿using System;
using System.Runtime.InteropServices;
using Medella.TdsClient.Exceptions;

namespace Medella.TdsClient.SNI.Internal
{
    public sealed class SniPacket : SafeHandle
    {
        internal SniPacket(SafeHandle sniHandle) : base(IntPtr.Zero, true)
        {
            SniNativeMethodWrapper.SNIPacketAllocate(sniHandle, SniNativeMethodWrapper.IOType.WRITE, ref handle);
            if (IntPtr.Zero == handle) throw SQL.SNIPacketAllocationFailure();
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