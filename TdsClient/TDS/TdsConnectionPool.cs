using System;
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
        private readonly ConcurrentQueue<TdsConnection> _freepool;
        private readonly SqlConnectionString _options;

        public TdsConnectionPool(SqlConnectionString options)
        {
            _options = options;
            _freepool = new ConcurrentQueue<TdsConnection>();
        }

        public TdsConnection GetConnection()
        {
            if (_freepool.TryDequeue(out var tdsController))
                return tdsController;

            var options = _options;
            var tdsStream= TdsStreamProxy.CreatedsStream(options.DataSource, options.ConnectTimeout);

            var cnn = new TdsConnection(tdsStream, options);
            return cnn;
        }

        public void Return(TdsConnection cnn)
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