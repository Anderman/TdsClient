using System.Collections.Concurrent;
using System.Collections.Generic;
using Medella.TdsClient.Cleanup;
using Medella.TdsClient.TDS.Controller;

namespace Medella.TdsClient.TDS
{
    public class Tds
    {
        private static readonly ConcurrentDictionary<string, TdsConnection> FreePool = new ConcurrentDictionary<string, TdsConnection>();

        public static TdsConnection GetConnection(string connectionString)
        {
            return FreePool.GetOrAdd(connectionString, x => new TdsConnection(new SqlConnectionString(connectionString)));
        }

        public static void Return(string connectionString, TdsPhysicalConnection tdsPhysicalConnection)
        {
            var freePool = FreePool.GetOrAdd(connectionString, x => new TdsConnection(new SqlConnectionString(connectionString)));
            freePool.Return( tdsPhysicalConnection);
        }
    }

    public class TdsConnection
    {
        private readonly Queue<TdsPhysicalConnection> _freepool;
        private readonly SqlConnectionString _options;
        private const int Capacity = 100;

        public TdsConnection(SqlConnectionString options)
        {
            _options = options;
            _freepool = new Queue<TdsPhysicalConnection>(Capacity);
        }

        public TdsPhysicalConnection GetConnection()
        {
            if (_freepool.TryDequeue(out var tdsController))
                return tdsController;
            var options = _options;
            var serverConnectionOptions = new ServerConnectionOptions(options.DataSource);
            var cnn = new TdsPhysicalConnection(serverConnectionOptions, options);
            return cnn;
        }
        public void Return(TdsPhysicalConnection cnn)
        {
            cnn.ResetToInitialState();
            _freepool.Enqueue(cnn);
        }
    }
}