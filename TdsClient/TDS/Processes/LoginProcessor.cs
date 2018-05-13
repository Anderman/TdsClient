using System;
using Medella.TdsClient.Cleanup;
using Medella.TdsClient.Contants;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.TDS.Controller;
using Medella.TdsClient.TDS.Messages.Client;
using Medella.TdsClient.TDS.Messages.Server;

namespace Medella.TdsClient.TDS.Processes
{
    public class LoginProcessor
    {
        private readonly SqlConnectionString _dbConnectionOptions;
        private readonly TdsPackage _package;

        /// <summary>
        ///     Responseble for maintaining the connection to the server.
        ///     Should be able to detect a failing connection and try to recover from a failed connection
        /// </summary>
        private readonly SessionData _recoverySessionData = null;


        public LoginProcessor(TdsPackage package, SqlConnectionString dbConnectionOptions)
        {
            _package = package;
            _dbConnectionOptions = dbConnectionOptions;

            Connect(package.Writer.InstanceName, dbConnectionOptions.MARS);
            Login();
        }

        private void Login()
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
                ClientToken = opt.IntegratedSecurity ? _package.Writer.GetClientToken(null) : null,
                RequestedFeatures = GetRequestedFeatures(opt)
            };
            _package.Writer.SendTdsLogin(login, _recoverySessionData);
            _package.Writer.SendLastMessage();
        }

        private static TdsEnums.FeatureExtension GetRequestedFeatures(SqlConnectionString opt)
        {
            // The GLOBALTRANSACTIONS feature is implicitly requested
            var requestedFeatures = TdsEnums.FeatureExtension.GlobalTransactions;
            if (opt.ConnectRetryCount > 0)
                requestedFeatures |= TdsEnums.FeatureExtension.SessionRecovery;
            return requestedFeatures;
        }

        public EncryptionOptions Connect(string instanceName, bool mars)
        {
            _package.Writer.SendPreLoginHandshake(instanceName, mars);
            var status = _package.Reader.ReadPackage();
            if (status != TdsEnums.ST_EOM)
                throw new Exception("Unexpected result returned from server after sending SendPreLoginHandshake");
            return _package.Reader.ParseConnect(EncryptionOptions.OFF);
        }

        public void VerifyToken(int tokenLength)
        {
            var token = _package.Reader.GetBytes(tokenLength);
            var clientToken = _package.Writer.GetClientToken(token);

            if (clientToken.Length != 0)
                _package.Writer.SendSspi(clientToken);
        }
    }
}