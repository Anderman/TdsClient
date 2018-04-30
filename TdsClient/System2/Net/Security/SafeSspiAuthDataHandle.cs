using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Medella.TdsClient.System2.Net.Security
{
    public sealed class SafeSspiAuthDataHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeSspiAuthDataHandle() : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return Interop.SspiCli.SspiFreeAuthIdentity(handle) == Interop.SECURITY_STATUS.OK;
        }
    }
}