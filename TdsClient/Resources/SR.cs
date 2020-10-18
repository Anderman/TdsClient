namespace System
{
    internal static class SR
    {
        internal static string GetString(string value) => value;

        internal static string GetString(string format, params object[] args) => Format(format, args);

        internal static string Format(string resourceFormat, params object[] args) =>
            args != null
                ? string.Format(resourceFormat, args)
                : resourceFormat;

        internal static string Format(string resourceFormat, object p1) => string.Format(resourceFormat, p1);

        internal static string Format(string resourceFormat, object p1, object p2) => string.Format(resourceFormat, p1, p2);

        internal static string Format(string resourceFormat, object p1, object p2, object p3) => string.Format(resourceFormat, p1, p2, p3);
    }
}