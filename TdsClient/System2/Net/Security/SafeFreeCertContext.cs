using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Medella.TdsClient.System2.Net.Security
{
    internal sealed class SafeFreeCertContext : SafeHandleZeroOrMinusOneIsInvalid
    {

        internal SafeFreeCertContext() : base(true) { }

        // This must be ONLY called from this file.
        internal void Set(IntPtr value)
        {
            this.handle = value;
        }

        private const uint CRYPT_ACQUIRE_SILENT_FLAG = 0x00000040;

        protected override bool ReleaseHandle()
        {
            Interop.Crypt32.CertFreeCertificateContext(handle);
            return true;
        }
    }
}