using System.Collections.Concurrent;
using System.Collections.Generic;
using Medella.TdsClient.Cleanup;

namespace Medella.TdsClient.TDS
{
    public class Tds
    {
        private const int Capacity = 100;
        private static readonly ConcurrentDictionary<string, Queue<Controller.TdsConnection>> FreePool = new ConcurrentDictionary<string, Queue<Controller.TdsConnection>>();

        public static Controller.TdsConnection GetConnection(string connectionString)
        {
            var freePool = FreePool.GetOrAdd(connectionString, x => new Queue<Controller.TdsConnection>(Capacity));
            if (freePool.TryDequeue(out var tdsController))
                return tdsController;

            var options = new SqlConnectionString(connectionString);
            var serverConnectionOptions = new ServerConnectionOptions(options.DataSource);
            var tdsStream = serverConnectionOptions.CreateTdsStream(options.ConnectTimeout);
            var cnn= new Controller.TdsConnection(tdsStream, options);
            return cnn;
        }

        public static void Return(string connectionString, Controller.TdsConnection tdsConnection)
        {
            var freePool = FreePool.GetOrAdd(connectionString, x => new Queue<Controller.TdsConnection>(Capacity));
            freePool.Enqueue(tdsConnection);

        }
    }
}