using System.Collections.Concurrent;
using Medella.TdsClient.Cleanup;
using Medella.TdsClient.TDS.Controller;

namespace Medella.TdsClient.TDS
{
    public class TdsConnectionPools
    {
        private static readonly ConcurrentDictionary<string, TdsConnectionPool> FreePool = new ConcurrentDictionary<string, TdsConnectionPool>();

        public static TdsConnectionPool GetConnectionPool(string connectionString)
        {
            return FreePool.GetOrAdd(connectionString, x => new TdsConnectionPool(new SqlConnectionString(connectionString)));
        }

        public static void Return(string connectionString, TdsConnection tdsConnection)
        {
            var freePool = FreePool.GetOrAdd(connectionString, x => new TdsConnectionPool(new SqlConnectionString(connectionString)));
            freePool.Return(tdsConnection);
        }
    }
}