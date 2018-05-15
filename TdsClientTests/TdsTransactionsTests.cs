using System;
using Medella.TdsClient.TDS;
using Medella.TdsClient.TDS.Processes;
using Xunit;

namespace TdsClientTests
{
    public class TdsTransactionsTests
    {
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=tmp;Trusted_Connection=True;";
        [Fact]
        public void Can_begin_transaction()
        {
            var cnn = TdsConnectionPools.GetConnectionPool(ConnectionString);
            using (var transaction = cnn.BeginTransaction())
            {
                transaction.ExecuteNonQuery("print 1");
                transaction.Commit();
            }
        }

        [Fact]
        public void Can_commit_transaction()
        {
            var guid = Guid.NewGuid();
            var cnn = TdsConnectionPools.GetConnectionPool(ConnectionString);
            using (var transaction = cnn.BeginTransaction())
            {
                transaction.ExecuteNonQuery($"CREATE TABLE [{guid}] (id int)");
                transaction.Commit();
            }

            cnn.ExecuteNonQuery($"DROP TABLE [{guid}]");
        }

        [Fact]
        public void Can_rollback_transaction()
        {
            var guid = Guid.NewGuid();
            var cnn = TdsConnectionPools.GetConnectionPool(ConnectionString);
            using (var transaction = cnn.BeginTransaction())
            {
                transaction.ExecuteNonQuery($"CREATE TABLE [{guid}] (id int)");
                transaction.Rollback();
            }

            cnn.ExecuteNonQuery($"if OBJECT_ID('{guid}') is not null RAISERROR (N'fatal',20,1) WITH LOG ");
        }

        [Fact]
        public void Can_AutoRollback_transaction()
        {
            var guid = Guid.NewGuid();
            var cnn = TdsConnectionPools.GetConnectionPool(ConnectionString);
            using (var transaction = cnn.BeginTransaction())
            {
                transaction.ExecuteNonQuery($"CREATE TABLE [{guid}] (id int)");
            }

            cnn.ExecuteNonQuery($"if OBJECT_ID('{guid}') is not null RAISERROR (N'fatal',20,1) WITH LOG ");
        }

        [Fact]
        public void Can_Rollback_OnsqlError_transaction()
        {
            var guid = Guid.NewGuid();
            var cnn = TdsConnectionPools.GetConnectionPool(ConnectionString);
            try
            {
                using (var transaction = cnn.BeginTransaction())
                {
                    transaction.ExecuteNonQuery($"RAISERROR (N'fatal',11,1) WITH LOG ");
                    transaction.Commit();
                }
            }
            catch (Exception)
            {
                // ignored
            }

            cnn.ExecuteNonQuery($"if OBJECT_ID('{guid}') is not null RAISERROR (N'fatal',20,1) WITH LOG ");
        }
    }
}