using System.Collections.Concurrent;
using Medella.TdsClient.Cleanup;
using Medella.TdsClient.TDS.Controller;

namespace Medella.TdsClient.TDS
{
    public class TdsClient
    {
        private static readonly ConcurrentDictionary<string, TdsConnectionPool> FreePool = new ConcurrentDictionary<string, TdsConnectionPool>();

        public static TdsConnectionPool GetConnection(string connectionString)
        {
            return FreePool.GetOrAdd(connectionString, x => new TdsConnectionPool(new SqlConnectionString(connectionString)));
        }

        public static void Return(string connectionString, TdsPhysicalConnection tdsPhysicalConnection)
        {
            var freePool = FreePool.GetOrAdd(connectionString, x => new TdsConnectionPool(new SqlConnectionString(connectionString)));
            freePool.Return(tdsPhysicalConnection);
        }
    }
}