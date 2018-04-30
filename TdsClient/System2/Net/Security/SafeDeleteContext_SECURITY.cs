using System.Runtime.InteropServices;

namespace Medella.TdsClient.System2.Net.Security
{
    internal sealed class SafeDeleteContext_SECURITY : SafeDeleteContext
    {
        internal SafeDeleteContext_SECURITY() : base() { }

        protected override bool ReleaseHandle()
        {
            if (this._EffectiveCredential != null)
            {
                this._EffectiveCredential.DangerousRelease();
            }

            return Interop.SspiCli.DeleteSecurityContext(ref _handle) == 0;
        }
    }
}