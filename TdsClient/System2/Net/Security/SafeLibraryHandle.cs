using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Medella.TdsClient.System2.Net.Security
{
    internal sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeLibraryHandle() : base(true)
        {
        }

        internal SafeLibraryHandle(bool ownsHandle) : base(ownsHandle)
        {
        }

        protected override bool ReleaseHandle()
        {
            return Interop.Kernel32.FreeLibrary(handle);
        }
    }
}