using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Medella.TdsClient.TDS.Controller;
using Medella.TdsClient.TDS.Messages.Client;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Package.Reader;
using Medella.TdsClient.TDS.Reader;

namespace Medella.TdsClient.TDS.Processes
{
    public static class ExecuteQueryExtenstions
    {
        private static readonly ConcurrentDictionary<string, Delegate> Readers = new ConcurrentDictionary<string, Delegate>();


        public static void ExecuteNonQuery(this TdsPhysicalConnection cnn, string text)
        {
            cnn.TdsPackage.Writer.SendExcuteBatch(text,cnn.SqlTransactionId);
            cnn.StreamParser.ParseInput();
        }


        public static List<T> ExecuteQuery<T>(this TdsPhysicalConnection cnn, string text) where T : class, new()
        {
            var writer = cnn.TdsPackage.Writer;
            var reader = cnn.TdsPackage.Reader;
            var parser = cnn.StreamParser;

            writer.SendExcuteBatch(text, cnn.SqlTransactionId);
            parser.ParseInput();
            if (parser.Status == ParseStatus.Done)
                return null;
            var result = new List<T>();
            var rowReader = GetRowReader<T>(reader, text);
            var columnReader = new TdsColumnReader(cnn.TdsPackage.Reader);
            while (parser.Status != ParseStatus.Done)
            {
                var row = rowReader(columnReader);
                parser.ParseInput();
                result.Add(row);
            }

            return result;
        }

        public static List<T> ExecuteParameterQuery<T>(this TdsPhysicalConnection cnn, FormattableString text) where T : class, new()
        {
            var writer = cnn.TdsPackage.Writer;
            var reader = cnn.TdsPackage.Reader;
            var parser = cnn.StreamParser;

            writer.SendRpc(reader.CurrentSession.DefaultCollation, text, cnn.SqlTransactionId);
            parser.ParseInput();
            if (parser.Status == ParseStatus.Done)
                return null;
            var r = new List<T>();
            var rowReader = GetRowReader<T>(reader, text.Format);
            var columnReader = new TdsColumnReader(reader);
            while (parser.Status != ParseStatus.Done)
            {
                var result = rowReader(columnReader);
                parser.ParseInput();
                r.Add(result);
            }

            return r;
        }

        private static Func<TdsColumnReader, T> GetRowReader<T>(TdsPackageReader reader, string key) where T : class, new()
        {
            return (Func<TdsColumnReader, T>)Readers.GetOrAdd(key, x => RowReader.GetComplexReader<T>(reader));
        }
    }
}