namespace Medella.TdsClient.Cleanup
{
    internal static class DbConnectionStringKeywords
    {
        // all
        // internal const string NamedConnection = "Named Connection";

        // SqlClient
        internal const string ApplicationIntent = "ApplicationIntent";
        internal const string ApplicationName = "Application Name";
        internal const string AsynchronousProcessing = "Asynchronous Processing";
        internal const string AttachDBFilename = "AttachDbFilename";
        internal const string ConnectTimeout = "Connect Timeout";
        internal const string ConnectionReset = "Connection Reset";
        internal const string ContextConnection = "Context Connection";
        internal const string CurrentLanguage = "Current Language";
        internal const string Encrypt = "Encrypt";
        internal const string FailoverPartner = "Failover Partner";
        internal const string InitialCatalog = "Initial Catalog";
        internal const string MultipleActiveResultSets = "MultipleActiveResultSets";
        internal const string MultiSubnetFailover = "MultiSubnetFailover";
        internal const string NetworkLibrary = "Network Library";
        internal const string PacketSize = "Packet Size";
        internal const string Replication = "Replication";
        internal const string TransactionBinding = "Transaction Binding";
        internal const string TrustServerCertificate = "TrustServerCertificate";
        internal const string TypeSystemVersion = "Type System Version";
        internal const string UserInstance = "User Instance";
        internal const string WorkstationID = "Workstation ID";
        internal const string ConnectRetryCount = "ConnectRetryCount";
        internal const string ConnectRetryInterval = "ConnectRetryInterval";

        // common keywords (OleDb, OracleClient, SqlClient)
        internal const string DataSource = "Data Source";
        internal const string IntegratedSecurity = "Integrated Security";
        internal const string Password = "Password";
        internal const string Driver = "Driver";
        internal const string PersistSecurityInfo = "Persist Security Info";
        internal const string UserID = "User ID";

        // managed pooling (OracleClient, SqlClient)
        internal const string Enlist = "Enlist";
        internal const string LoadBalanceTimeout = "Load Balance Timeout";
        internal const string MaxPoolSize = "Max Pool Size";
        internal const string Pooling = "Pooling";
        internal const string MinPoolSize = "Min Pool Size";
    }
}