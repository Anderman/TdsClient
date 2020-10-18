namespace Medella.TdsClient.Constants
{
    internal static class DbConnectionStringDefaults
    {
        // all
        // internal const string NamedConnection = "";

        // SqlClient
        internal const ApplicationIntent ApplicationIntent = Constants.ApplicationIntent.ReadWrite;
        internal const string ApplicationName = "Core .Net SqlClient Data Provider";
        internal const string AttachDBFilename = "";
        internal const int ConnectTimeout = 15;
        internal const string CurrentLanguage = "";
        internal const string DataSource = "";
        internal const bool Encrypt = false;
        internal const bool Enlist = true;
        internal const string FailoverPartner = "";
        internal const string InitialCatalog = "";
        internal const bool IntegratedSecurity = false;
        internal const int LoadBalanceTimeout = 0; // default of 0 means don't use
        internal const bool MultipleActiveResultSets = false;
        internal const bool MultiSubnetFailover = false;
        internal const int MaxPoolSize = 100;
        internal const int MinPoolSize = 0;
        internal const int PacketSize = 8000;
        internal const string Password = "";
        internal const bool PersistSecurityInfo = false;
        internal const bool Pooling = true;
        internal const bool TrustServerCertificate = false;
        internal const string TypeSystemVersion = "Latest";
        internal const string UserID = "";
        internal const bool UserInstance = false;
        internal const bool Replication = false;
        internal const string WorkstationID = "";
        internal const string TransactionBinding = "Implicit Unbind";
        internal const int ConnectRetryCount = 1;
        internal const int ConnectRetryInterval = 10;
    }
}