using System;
using System.Collections.Generic;
using System.Text;
using Medella.TdsClient.Cleanup;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.SNI;
using Medella.TdsClient.TDS.Processes;

namespace Medella.TdsClient.TDS.Controller
{
    public enum ParseStatus
    {
        Row,
        Done,
        AltRow,
        Unknown,
        BeforeFirstColumn,
        AfterLastColumn
    }

    public class TdsPhysicalConnection :IDisposable
    {
        private readonly string _connectionString;
        private readonly int _messageCountAfterlogin;
        public readonly TdsStreamParser StreamParser;
        public readonly TdsPackage TdsPackage;
        private ISniHandle _tdsStream;

        static TdsPhysicalConnection()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public TdsPhysicalConnection(ServerConnectionOptions serverConnectionOptions, SqlConnectionString dbConnectionOptions)
        {
            _connectionString = dbConnectionOptions.ConnectionString;
            _tdsStream = serverConnectionOptions.CreateTdsStream(dbConnectionOptions.ConnectTimeout);

            TdsPackage = new TdsPackage(_tdsStream);
            var loginProcessor = new LoginProcessor(TdsPackage, dbConnectionOptions);
            StreamParser = new TdsStreamParser(TdsPackage, loginProcessor);
            StreamParser.ParseInput();
            _messageCountAfterlogin = SqlMessages.Count;
        }

        public List<SqlInfoAndError> SqlMessages => TdsPackage.Reader.CurrentSession.Errors;

        public void ResetToInitialState()
        {
            // Oeps we are responsible for returning clean to the pool!! The pool should clone the connectionstate.
            // Fix later when this become complex
            if (StreamParser.Status == ParseStatus.Done) //Don't return connections to the pool that have problems
            {
                if (SqlMessages.Count > _messageCountAfterlogin)
                    SqlMessages.RemoveAt(_messageCountAfterlogin - 1);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}