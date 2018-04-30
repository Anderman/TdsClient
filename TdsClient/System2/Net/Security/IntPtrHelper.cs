using System;

namespace Medella.TdsClient.System2.Net.Security
{
    internal static class IntPtrHelper
    {
        internal static IntPtr Add(IntPtr a, int b) => (IntPtr)((long)a + (long)b);
    }
}