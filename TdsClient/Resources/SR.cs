using System.Resources;
using System.Runtime.CompilerServices;
using Medella.TdsClient.Resources;

namespace System
{
    internal static class SR
    {
        internal static string GetString(string value)
        {
            return value;
        }

        internal static string GetString(string format, params object[] args)
        {
            return Format(format, args);
        }

        internal static string Format(string resourceFormat, params object[] args)
        {
            return args != null 
                ? string.Format(resourceFormat, args) 
                : resourceFormat;
        }

        internal static string Format(string resourceFormat, object p1)
        {
            return string.Format(resourceFormat, p1);
        }

        internal static string Format(string resourceFormat, object p1, object p2)
        {
            return string.Format(resourceFormat, p1, p2);
        }

        internal static string Format(string resourceFormat, object p1, object p2, object p3)
        {
            return string.Format(resourceFormat, p1, p2, p3);
        }
    }
}