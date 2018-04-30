using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Medella.TdsClient.Cleanup;
using Medella.TdsClient.Contants;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.SNI.SniNp;
using Medella.TdsClient.System2;
using Medella.TdsClient.TDS.Messages.Client;
using Medella.TdsClient.TDS.Messages.Server;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Reader;
using SqlClient;

namespace Medella.TdsClient.TDS
{
    public class Tds
    {
        private static readonly ConcurrentDictionary<string, Delegate> Readers = new ConcurrentDictionary<string, Delegate>();
        private readonly SqlConnectionString _dbConnectionOptions;
        private readonly TdsPackageReader _reader;
        private readonly SessionData _recoverySessionData;
        private readonly ServerConnectionOptions _serverConnectionOptions;
        private readonly TdsController _tdsController;
        private readonly TdsPackageWriter _writer;
        private TdsConnection _tdsConnection;

        public Tds(string connectionString)
        {
            _recoverySessionData = null;
            _dbConnectionOptions = new SqlConnectionString(connectionString);
            ;
            _serverConnectionOptions = new ServerConnectionOptions(_dbConnectionOptions.DataSource, _dbConnectionOptions.IntegratedSecurity);
            var sniHandle = new SniNpHandle(_serverConnectionOptions.PipeServerName, _serverConnectionOptions.PipeName, 15);

            _reader = new TdsPackageReader(sniHandle);
            _writer = new TdsPackageWriter(sniHandle);
            _tdsController = new TdsController(_reader, _writer, _serverConnectionOptions.Sspi);
        }

        public void Login()
        {
            var opt = _dbConnectionOptions;
            var login = new SqlLogin
            {
                userInstance = opt.UserInstance,
                hostName = opt.ObtainWorkstationId(),
                userName = opt.UserID,
                password = opt.Password,
                applicationName = opt.ApplicationName,
                language = opt.CurrentLanguage,
                // Do not send attachdbfilename or database to SSE primary instance
                database = opt.UserInstance ? null : opt.InitialCatalog,
                attachDBFilename = opt.UserInstance ? null : opt.AttachDBFilename,
                serverName = opt.DataSource,
                useReplication = opt.Replication,
                useSSPI = opt.IntegratedSecurity,
                packetSize = opt.PacketSize,
                ReadOnlyIntent = opt.ApplicationIntent == ApplicationIntent.ReadOnly,
                Sspi = opt.IntegratedSecurity ? _serverConnectionOptions.Sspi : null,
                RequestedFeatures = GetRequestedFeatures(opt)
            };
            _writer.SendTdsLogin(login, _recoverySessionData);
            _writer.FlushBuffer();

            _tdsController.ParseInput();
        }

        public void ExecuteNonQuery(string text)
        {
            WriterExcuteBatch.Create(_writer, text);
            _tdsController.ParseInput();
        }

        public List<T> ExecuteQuery<T>(FormattableString text) where T : class, new()
        {
            return ExecuteQuery<T>(text.ToString(), text.Format);
        }
        public List<T> ExecuteQuery<T>(string text) where T : class, new()
        {
            return ExecuteQuery<T>(text, text);
        }
        private List<T> ExecuteQuery<T>(string query, string key) where T : class, new()
        {
            WriterExcuteBatch.Create(_writer, query);
            _tdsController.ParseInput();
            if (_tdsController.Status == ParseStatus.Done)
                return null;
            var r = new List<T>();
            var rowReader = GetRowReader<T>(key);
            var columnReader = new TdsColumnReader(_reader);
            while (_tdsController.Status != ParseStatus.Done)
            {
                var result = rowReader(columnReader);
                _tdsController.ParseInput();
                r.Add(result);
            }

            return r;
        }

        public bool Read()
        {
            while (true)
            {
                if (_tdsController.Status == ParseStatus.Row) return true;
                if (_tdsController.Status == ParseStatus.Done) return false;
                _tdsController.ParseInput();
            }
        }

        public Func<TdsColumnReader, T> GetRowReader<T>(string key) where T : class, new()
        {
            return (Func<TdsColumnReader, T>) Readers.GetOrAdd(key, x => RowReader.GetComplexReader<T>(_reader));
        }


        private static TdsEnums.FeatureExtension GetRequestedFeatures(SqlConnectionString opt)
        {
            // The GLOBALTRANSACTIONS feature is implicitly requested
            var requestedFeatures = TdsEnums.FeatureExtension.GlobalTransactions;
            if (opt.ConnectRetryCount > 0)
                requestedFeatures |= TdsEnums.FeatureExtension.SessionRecovery;
            return requestedFeatures;
        }

        public void Connect()
        {
            SendPreLoginHandshake();
            _tdsConnection = ConsumePreLoginHandshake();
        }

        private TdsConnection ConsumePreLoginHandshake()
        {
            var status = _reader.ReadPackage();
            if (status != TdsEnums.ST_EOM)
                throw new Exception("Unexpected result returned from server after sending SendPreLoginHandshake");
            return _reader.ParseConnect(EncryptionOptions.OFF);
        }

        private void SendPreLoginHandshake()
        {
            _writer.SendPreLoginHandshake(_serverConnectionOptions.InstanceNameBytes, _dbConnectionOptions.MARS);
        }
    }
}