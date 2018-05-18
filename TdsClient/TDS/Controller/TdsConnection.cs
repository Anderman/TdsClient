using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Medella.TdsClient.Cleanup;
using Medella.TdsClient.TdsStream;
using Medella.TdsClient.TDS.Processes;

namespace Medella.TdsClient.TDS.Controller
{
    public class TdsConnection : IDisposable
    {
        private readonly int _messageCountAfterlogin;
        private readonly ITdsStream _tdsStream;
        public readonly TdsStreamParser StreamParser;
        public readonly TdsPackage TdsPackage;

        static TdsConnection()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public TdsConnection(ITdsStream tdsStream, SqlConnectionString dbConnectionOptions)
        {
            _tdsStream = tdsStream;
            TdsPackage = new TdsPackage(_tdsStream);
            var loginProcessor = new LoginProcessor(TdsPackage, dbConnectionOptions);
            StreamParser = new TdsStreamParser(TdsPackage, loginProcessor);
            StreamParser.ParseInput();
            _messageCountAfterlogin = SqlMessages.Count;
        }

        public long SqlTransactionId { get; set; }

        public List<SqlInfoAndError> SqlMessages => TdsPackage.Reader.CurrentSession.Errors;

        public void Dispose()
        {
            Debug.WriteLine($"Dispose connection");
            _tdsStream.Dispose();
        }

        public void ResetToInitialState()
        {
            if (SqlMessages.Count > _messageCountAfterlogin)
                SqlMessages.RemoveAt(_messageCountAfterlogin - 1);
            SqlTransactionId = 0;
            if (!TdsPackage.Reader.IsFinsched())
                throw new Exception("reader not finised");
        }
    }
}