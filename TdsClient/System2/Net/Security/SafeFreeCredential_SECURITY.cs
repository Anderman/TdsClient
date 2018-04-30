using System.Runtime.InteropServices;

namespace Medella.TdsClient.System2.Net.Security
{
    internal sealed class SafeFreeCredential_SECURITY : SafeFreeCredentials
    {
        public SafeFreeCredential_SECURITY() : base() { }

        protected override bool ReleaseHandle()
        {
            return Interop.SspiCli.FreeCredentialsHandle(ref _handle) == 0;
        }
    }
}