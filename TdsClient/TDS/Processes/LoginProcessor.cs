using Medella.TdsClient.Cleanup;
using Medella.TdsClient.Constants;
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
            var login = new LoginOptions
            {
                UserInstance = opt.UserInstance,
                HostName = opt.ObtainWorkstationId(),
                UserName = opt.UserID,
                Password = opt.Password,
                ApplicationName = opt.ApplicationName,
                Language = opt.CurrentLanguage,
                // Do not send attachdbfilename or database to SSE primary instance
                Database = opt.UserInstance ? null : opt.InitialCatalog,
                AttachDbFilename = opt.UserInstance ? null : opt.AttachDBFilename,
                ServerName = opt.DataSource,
                UseReplication = opt.Replication,
                UseSspi = opt.IntegratedSecurity,
                PacketSize = opt.PacketSize,
                ReadOnlyIntent = opt.ApplicationIntent == ApplicationIntent.ReadOnly,
                ClientToken = opt.IntegratedSecurity ? _package.Reader.GetClientToken(null) : null,
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
            _package.Reader.CheckBuffer(8);
            return _package.Reader.ParseConnect(EncryptionOptions.OFF);
        }

        public void VerifyToken(int tokenLength)
        {
            var token = _package.Reader.GetBytes(tokenLength);
            var clientToken = _package.Reader.GetClientToken(token);

            if (clientToken.Length != 0)
                _package.Writer.SendSspi(clientToken);
        }
    }
}