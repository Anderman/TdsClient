using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Medella.TdsClient.TDS.Processes
{
    public static class ConnectionPoolExtensions
    {
        public static async Task ExecuteNonQueryAsync(this TdsConnectionPool tdsConnectionPool, string text) => await Task.Run(() => ExecuteNonQuery(tdsConnectionPool, text));

        public static void ExecuteNonQuery(this TdsConnectionPool tdsConnectionPool, string text)
        {
            var cnn = tdsConnectionPool.GetConnection();
            cnn.ExecuteNonQuery(text);
            tdsConnectionPool.Return(cnn);
        }

        public static async Task<List<T>> ExecuteQueryAsync<T>(this TdsConnectionPool tdsConnectionPool, string text) where T : class, new() => await Task.Run(() => ExecuteQuery<T>(tdsConnectionPool, text));

        public static List<T> ExecuteQuery<T>(this TdsConnectionPool tdsConnectionPool, string text) where T : class, new()
        {
            var cnn = tdsConnectionPool.GetConnection();
            var result = cnn.ExecuteQuery<T>(text);
            tdsConnectionPool.Return(cnn);
            return result;
        }

        public static async Task<List<T>> ExecuteParameterQueryASync<T>(this TdsConnectionPool tdsConnectionPool, FormattableString text) where T : class, new() => await Task.Run(() => ExecuteParameterQuery<T>(tdsConnectionPool, text));

        public static List<T> ExecuteParameterQuery<T>(this TdsConnectionPool tdsConnectionPool, FormattableString text) where T : class, new()
        {
            var cnn = tdsConnectionPool.GetConnection();
            var result = cnn.ExecuteParameterQuery<T>(text);
            tdsConnectionPool.Return(cnn);
            return result;
        }

        public static async Task BulkInsertAsync<T>(this TdsConnectionPool tdsConnectionPool, IEnumerable<T> objects, string tableName, Dictionary<string, PropertyInfo> columnMapping)
        {
            await Task.Run(() => BulkInsert(tdsConnectionPool, objects, tableName, columnMapping));
        }

        public static async Task BulkInsertAsync<T>(this TdsConnectionPool tdsConnectionPool, IEnumerable<T> objects, string tableName)
        {
            await Task.Run(() => BulkInsert(tdsConnectionPool, objects, tableName));
        }

        public static void BulkInsert<T>(this TdsConnectionPool tdsConnectionPool, IEnumerable<T> objects, string tableName)
        {
            var cnn = tdsConnectionPool.GetConnection();
            cnn.BulkInsert(objects, tableName);
            tdsConnectionPool.Return(cnn);
        }

        public static void BulkInsert<T>(this TdsConnectionPool tdsConnectionPool, IEnumerable<T> objects, string tableName, Dictionary<string, PropertyInfo> columnMapping)
        {
            var cnn = tdsConnectionPool.GetConnection();
            cnn.BulkInsert(objects, tableName, columnMapping);
            tdsConnectionPool.Return(cnn);
        }
    }
}