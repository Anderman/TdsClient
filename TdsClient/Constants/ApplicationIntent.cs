using System;

namespace Medella.TdsClient.Constants
{
    public enum ApplicationIntent
    {
        ReadWrite = 0,
        ReadOnly = 1
    }

    internal class SqlUdtInfo
    {
        public static object TryGetFromType(Type dataType) => throw new NotImplementedException();
    }

    internal class SqlDataRecord
    {
    }
}