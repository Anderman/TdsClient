using System.Collections.Concurrent;
using System.Diagnostics;
using Medella.TdsClient.Cleanup;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TdsStream;
using Medella.TdsClient.TDS.Controller;

namespace Medella.TdsClient.TDS
{
    public class TdsConnectionPool
    {
        private readonly ConcurrentQueue<TdsPhysicalConnection> _freepool;
        private readonly SqlConnectionString _options;
        private const int Capacity = 100;

        public TdsConnectionPool(SqlConnectionString options)
        {
            _options = options;
            _freepool = new ConcurrentQueue<TdsPhysicalConnection>();
        }

        public TdsPhysicalConnection GetConnection()
        {
            if (_freepool.TryDequeue(out var tdsController))
                return tdsController;
            var options = _options;
            var serverConnectionOptions = new TdsStreamProxy(options.DataSource);
            var cnn = new TdsPhysicalConnection(serverConnectionOptions, options);
            return cnn;
        }
        public void Return(TdsPhysicalConnection cnn)
        {
            Debug.WriteLine($"connection returned to the pool");
            cnn.ResetToInitialState();
            _freepool.Enqueue(cnn);
        }

        public TdsTransaction BeginTransaction() => BeginTransaction(TdsEnums.TransactionManagerIsolationLevel.Unspecified);
        public TdsTransaction BeginTransaction(TdsEnums.TransactionManagerIsolationLevel isolationLevel)
        {
            return new TdsTransaction(this,isolationLevel);
        }
    }
}