using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Medella.TdsClient.Cleanup;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.TdsStream;
using Medella.TdsClient.TDS.Processes;

namespace Medella.TdsClient.TDS.Controller
{
    public class TdsPhysicalConnection : IDisposable
    {
        private readonly int _messageCountAfterlogin;
        public long SqlTransactionId { get; set; }
        public readonly TdsStreamParser StreamParser;
        public readonly TdsPackage TdsPackage;
        private readonly ITdsStream _tdsStream;

        static TdsPhysicalConnection()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public TdsPhysicalConnection(TdsStreamProxy tdsStreamProxy, SqlConnectionString dbConnectionOptions)
        {
            _tdsStream = tdsStreamProxy.CreateTdsStream(dbConnectionOptions.ConnectTimeout);

            TdsPackage = new TdsPackage(_tdsStream);
            var loginProcessor = new LoginProcessor(TdsPackage, dbConnectionOptions);
            StreamParser = new TdsStreamParser(TdsPackage, loginProcessor);
            StreamParser.ParseInput();
            _messageCountAfterlogin = SqlMessages.Count;
        }

        public List<SqlInfoAndError> SqlMessages => TdsPackage.Reader.CurrentSession.Errors;

        public void ResetToInitialState()
        {
            if (SqlMessages.Count > _messageCountAfterlogin)
                SqlMessages.RemoveAt(_messageCountAfterlogin - 1);
            SqlTransactionId = 0;
        }

        public void Dispose()
        {
            Debug.WriteLine($"Dispose connection");
            _tdsStream.Dispose();
        }
    }
}