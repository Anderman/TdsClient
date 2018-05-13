using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Controller;
using Medella.TdsClient.TDS.Messages.Client;
using Medella.TdsClient.TDS.Messages.Server.Environment;
using Medella.TdsClient.TDS.Processes;

namespace Medella.TdsClient.TDS
{
    public class TdsTransaction : IDisposable
    {
        private List<TdsPhysicalConnection> _openTransactions = new List<TdsPhysicalConnection>();
        private readonly TdsEnums.TransactionManagerIsolationLevel _isolationLevel;
        private readonly TdsConnectionPool _tdsConnectionPool;

        public TdsTransaction(TdsConnectionPool tdsConnectionPool, TdsEnums.TransactionManagerIsolationLevel isolationLevel)
        {
            _tdsConnectionPool = tdsConnectionPool;
            _isolationLevel = isolationLevel;
        }

        public void Commit()
        {
            foreach (var openTransaction in _openTransactions)
            {
                var writer = openTransaction.TdsPackage.Writer;
                var parser = openTransaction.StreamParser;
                writer.SendTransactionCommit(openTransaction.SqlTransactionId);
                parser.ParseInput();
                _tdsConnectionPool.Return(openTransaction);
            }
            _openTransactions= new List<TdsPhysicalConnection>();
            
        }

        public void Rollback()
        {
            foreach (var openTransaction in _openTransactions)
            {
                //var writer = openTransaction.Connection.TdsPackage.Writer;
                //var parser = openTransaction.Connection.StreamParser;
                //writer.SendTransactionRollback(openTransaction.SqlTransactionId);
                //parser.ParseInput();
                openTransaction.Dispose();
                //_tdsConnectionPool.Return(openTransaction.Connection);
            }

            _openTransactions = new List<TdsPhysicalConnection>();
        }

        public void Dispose()
        {
            Rollback();
            _openTransactions = null;
        }

        public async Task ExecuteNonQueryAsync(string text) => await Task.Run(() => ExecuteNonQuery(text));
        public void ExecuteNonQuery(string text)
        {
            var cnn = StartTransaction();
            cnn.ExecuteNonQuery(text);
        }

        public  async Task<List<T>> ExecuteQueryAsync<T>(string text) where T : class, new() => await Task.Run(() => ExecuteQuery<T>(text));
        public List<T> ExecuteQuery<T>(string text) where T : class, new()
        {
            var cnn = StartTransaction();
            return cnn.ExecuteQuery<T>(text);
        }

        public async Task<List<T>> ExecuteParameterQueryASync<T>(TdsConnectionPool tdsConnectionPool, FormattableString text) where T : class, new() => await Task.Run(() => ExecuteParameterQuery<T>(tdsConnectionPool, text));
        public List<T> ExecuteParameterQuery<T>(TdsConnectionPool tdsConnectionPool, FormattableString text) where T : class, new()
        {
            var cnn = StartTransaction();
            return cnn.ExecuteParameterQuery<T>(text);
        }

        private TdsPhysicalConnection StartTransaction()
        {
            var cnn = _tdsConnectionPool.GetConnection();
            cnn.TdsPackage.Writer.SendTransactionBegin(_isolationLevel);
            var sqlTransactionId = 0L;
            cnn.StreamParser.ParseInput(x => { sqlTransactionId = cnn.TdsPackage.Reader.EnvSqlTransaction(); });
            cnn.SqlTransactionId=sqlTransactionId;
            _openTransactions.Add(cnn);
            return cnn;
        }
    }
}