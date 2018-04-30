using System.Runtime.InteropServices;

namespace Medella.TdsClient.System2.Net.Security
{
    internal sealed class SafeFreeContextBuffer_SECURITY : SafeFreeContextBuffer
    {
        internal SafeFreeContextBuffer_SECURITY() : base() { }

        protected override bool ReleaseHandle()
        {
            return Interop.SspiCli.FreeContextBuffer(handle) == 0;
        }
    }
}