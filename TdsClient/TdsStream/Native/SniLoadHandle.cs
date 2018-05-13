using System;
using System.Runtime.InteropServices;
using Medella.TdsClient.Contants;

namespace Medella.TdsClient.TdsStream.Native
{
    public class SniLoadHandle : SafeHandle
    {
        public static readonly SniLoadHandle SingletonInstance = new SniLoadHandle();

        private SniLoadHandle() : base(IntPtr.Zero, true)
        {
            SniStatus = SniNativeMethodWrapper.SNIInitialize();
            if (TdsEnums.SNI_SUCCESS != SniStatus)
                throw new Exception("Failed to Load spi");
            handle = (IntPtr) 1; // Initialize to non-zero dummy variable.
        }

        public uint SniStatus { get; }
        public override bool IsInvalid => TdsEnums.SNI_SUCCESS != SniStatus;

        protected override bool ReleaseHandle()
        {
            if (handle == IntPtr.Zero)
                return true;
            if (TdsEnums.SNI_SUCCESS == SniStatus) SniNativeMethodWrapper.SNITerminate();
            handle = IntPtr.Zero;
            return true;
        }
    }
}