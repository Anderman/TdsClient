using System.Resources;
using System.Runtime.CompilerServices;
using Medella.TdsClient.Resources;

namespace System
{
    internal static class SR
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool UsingResourceKeys()
        {
            return false;
        }
        internal static string GetString(string value)
        {
            return value;
        }

        internal static string GetResourceString(string resourceKey, string defaultString)
        {
            string resourceString = null;
            try
            {
                resourceString = Strings.ResourceManager.GetString(resourceKey);
            }
            catch (MissingManifestResourceException)
            {
            }

            if (defaultString != null && resourceKey.Equals(resourceString, StringComparison.Ordinal)) return defaultString;

            return resourceString;
        }

        internal static string GetString(string format, params object[] args)
        {
            return Format(format, args);
        }

        internal static string Format(string resourceFormat, params object[] args)
        {
            if (args != null)
            {
                if (UsingResourceKeys()) return resourceFormat + string.Join(", ", args);

                return string.Format(resourceFormat, args);
            }

            return resourceFormat;
        }

        internal static string Format(string resourceFormat, object p1)
        {
            if (UsingResourceKeys()) return string.Join(", ", resourceFormat, p1);

            return string.Format(resourceFormat, p1);
        }

        internal static string Format(string resourceFormat, object p1, object p2)
        {
            if (UsingResourceKeys()) return string.Join(", ", resourceFormat, p1, p2);

            return string.Format(resourceFormat, p1, p2);
        }

        internal static string Format(string resourceFormat, object p1, object p2, object p3)
        {
            if (UsingResourceKeys()) return string.Join(", ", resourceFormat, p1, p2, p3);

            return string.Format(resourceFormat, p1, p2, p3);
        }
    }
}