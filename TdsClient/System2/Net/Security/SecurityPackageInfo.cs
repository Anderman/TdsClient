using System;
using System.Runtime.InteropServices;

namespace Medella.TdsClient.System2.Net.Security
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SecurityPackageInfo
    {
        // see SecPkgInfoW in <sspi.h>
        internal int Capabilities;
        internal short Version;
        internal short RPCID;
        internal int MaxToken;
        internal IntPtr Name;
        internal IntPtr Comment;

        internal static readonly int Size = Marshal.SizeOf<SecurityPackageInfo>();
        internal static readonly int NameOffest = (int)Marshal.OffsetOf<SecurityPackageInfo>("Name");
    }
}