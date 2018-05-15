using System;
using System.Collections.Generic;
using System.Diagnostics;
using Medella.TdsClient.Contants;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.LocalDb;

namespace Medella.TdsClient.Cleanup
{
    public sealed class SqlConnectionString : DbConnectionOptions
    {
        public enum TransactionBindingEnum
        {
            ImplicitUnbind,
            ExplicitUnbind
        }


        public enum TypeSystem
        {
            Latest = 2008,
            SQLServer2000 = 2000,
            SQLServer2005 = 2005,
            SQLServer2008 = 2008,
            SQLServer2012 = 2012
        }

        internal const int SynonymCount = 18;
        internal const int DeprecatedSynonymCount = 3;

        private static Dictionary<string, string> s_sqlClientSynonyms;
        private static readonly Version constTypeSystemAsmVersion10 = new Version("10.0.0.0");
        private static readonly Version constTypeSystemAsmVersion11 = new Version("11.0.0.0");

        public SqlConnectionString(string connectionString) : base(connectionString, GetParseSynonyms())
        {
            ThrowUnsupportedIfKeywordSet(KEY.AsynchronousProcessing);
            ThrowUnsupportedIfKeywordSet(KEY.Connection_Reset);
            ThrowUnsupportedIfKeywordSet(KEY.Context_Connection);

            // Network Library has its own special error message
            if (ContainsKey(KEY.Network_Library)) throw SQL.NetworkLibraryKeywordNotSupported();
            ConnectionString = connectionString;

            IntegratedSecurity = ConvertValueToIntegratedSecurity();
            Encrypt = ConvertValueToBoolean(KEY.Encrypt, DEFAULT.Encrypt);
            Enlist = ConvertValueToBoolean(KEY.Enlist, DEFAULT.Enlist);
            MARS = ConvertValueToBoolean(KEY.MARS, DEFAULT.MARS);
            PersistSecurityInfo = ConvertValueToBoolean(KEY.Persist_Security_Info, DEFAULT.Persist_Security_Info);
            Pooling = ConvertValueToBoolean(KEY.Pooling, DEFAULT.Pooling);
            Replication = ConvertValueToBoolean(KEY.Replication, DEFAULT.Replication);
            UserInstance = ConvertValueToBoolean(KEY.User_Instance, DEFAULT.User_Instance);
            MultiSubnetFailover = ConvertValueToBoolean(KEY.MultiSubnetFailover, DEFAULT.MultiSubnetFailover);

            ConnectTimeout = ConvertValueToInt32(KEY.Connect_Timeout, DEFAULT.ConnectTimeout);
            LoadBalanceTimeout = ConvertValueToInt32(KEY.Load_Balance_Timeout, DEFAULT.LoadBalanceTimeout);
            MaxPoolSize = ConvertValueToInt32(KEY.Max_Pool_Size, DEFAULT.Max_Pool_Size);
            MinPoolSize = ConvertValueToInt32(KEY.Min_Pool_Size, DEFAULT.Min_Pool_Size);
            PacketSize = ConvertValueToInt32(KEY.Packet_Size, DEFAULT.Packet_Size);
            ConnectRetryCount = ConvertValueToInt32(KEY.Connect_Retry_Count, DEFAULT.Connect_Retry_Count);
            ConnectRetryInterval = ConvertValueToInt32(KEY.Connect_Retry_Interval, DEFAULT.Connect_Retry_Interval);

            ApplicationIntent = ConvertValueToApplicationIntent();
            ApplicationName = ConvertValueToString(KEY.Application_Name, DEFAULT.ApplicationName);
            AttachDBFilename = ConvertValueToString(KEY.AttachDBFilename, DEFAULT.AttachDbFilename);
            CurrentLanguage = ConvertValueToString(KEY.Current_Language, DEFAULT.CurrentLanguage);
            DataSource = ConvertValueToString(KEY.Data_Source, DEFAULT.DataSource);
            //LocalDBInstance = LocalDbApi.GetLocalDbInstanceNameFromServerName(DataSource);
            FailoverPartner = ConvertValueToString(KEY.FailoverPartner, DEFAULT.FailoverPartner);
            InitialCatalog = ConvertValueToString(KEY.Initial_Catalog, DEFAULT.InitialCatalog);
            Password = ConvertValueToString(KEY.Password, DEFAULT.Password);
            TrustServerCertificate = ConvertValueToBoolean(KEY.TrustServerCertificate, DEFAULT.TrustServerCertificate);

            // Temporary string - this value is stored internally as an enum.
            var typeSystemVersionString = ConvertValueToString(KEY.Type_System_Version, null);
            var transactionBindingString = ConvertValueToString(KEY.TransactionBinding, null);

            UserID = ConvertValueToString(KEY.User_ID, DEFAULT.User_ID);
            WorkstationId = ConvertValueToString(KEY.Workstation_Id, null);


            if (LoadBalanceTimeout < 0) throw ADP.InvalidConnectionOptionValue(KEY.Load_Balance_Timeout);

            if (ConnectTimeout < 0) throw ADP.InvalidConnectionOptionValue(KEY.Connect_Timeout);

            if (MaxPoolSize < 1) throw ADP.InvalidConnectionOptionValue(KEY.Max_Pool_Size);

            if (MinPoolSize < 0) throw ADP.InvalidConnectionOptionValue(KEY.Min_Pool_Size);
            if (MaxPoolSize < MinPoolSize) throw ADP.InvalidMinMaxPoolSizeValues();

            if (PacketSize < TdsEnums.MIN_PACKET_SIZE || TdsEnums.MAX_PACKET_SIZE < PacketSize) throw SQL.InvalidPacketSizeValue();


            ValidateValueLength(ApplicationName, TdsEnums.MAXLEN_APPNAME, KEY.Application_Name);
            ValidateValueLength(CurrentLanguage, TdsEnums.MAXLEN_LANGUAGE, KEY.Current_Language);
            ValidateValueLength(DataSource, TdsEnums.MAXLEN_SERVERNAME, KEY.Data_Source);
            ValidateValueLength(FailoverPartner, TdsEnums.MAXLEN_SERVERNAME, KEY.FailoverPartner);
            ValidateValueLength(InitialCatalog, TdsEnums.MAXLEN_DATABASE, KEY.Initial_Catalog);
            ValidateValueLength(Password, TdsEnums.MAXLEN_PASSWORD, KEY.Password);
            ValidateValueLength(UserID, TdsEnums.MAXLEN_USERNAME, KEY.User_ID);
            if (null != WorkstationId) ValidateValueLength(WorkstationId, TdsEnums.MAXLEN_HOSTNAME, KEY.Workstation_Id);

            if (!string.Equals(DEFAULT.FailoverPartner, FailoverPartner, StringComparison.OrdinalIgnoreCase))
            {
                // fail-over partner is set

                if (MultiSubnetFailover) throw SQL.MultiSubnetFailoverWithFailoverPartner(false, null);

                if (string.Equals(DEFAULT.InitialCatalog, InitialCatalog, StringComparison.OrdinalIgnoreCase)) throw ADP.MissingConnectionOptionValue(KEY.FailoverPartner, KEY.Initial_Catalog);
            }

            if (0 <= AttachDBFilename.IndexOf('|'))
                throw ADP.InvalidConnectionOptionValue(KEY.AttachDBFilename);
            ValidateValueLength(AttachDBFilename, TdsEnums.MAXLEN_ATTACHDBFILE, KEY.AttachDBFilename);

            TypeSystemAssemblyVersion = constTypeSystemAsmVersion10;

            if (UserInstance && !string.IsNullOrEmpty(FailoverPartner)) throw SQL.UserInstanceFailoverNotCompatible();

            if (string.IsNullOrEmpty(typeSystemVersionString)) typeSystemVersionString = DbConnectionStringDefaults.TypeSystemVersion;

            if (typeSystemVersionString.Equals(TYPESYSTEMVERSION.Latest, StringComparison.OrdinalIgnoreCase))
            {
                TypeSystemVersion = TypeSystem.Latest;
            }
            else if (typeSystemVersionString.Equals(TYPESYSTEMVERSION.SQL_Server_2000, StringComparison.OrdinalIgnoreCase))
            {
                TypeSystemVersion = TypeSystem.SQLServer2000;
            }
            else if (typeSystemVersionString.Equals(TYPESYSTEMVERSION.SQL_Server_2005, StringComparison.OrdinalIgnoreCase))
            {
                TypeSystemVersion = TypeSystem.SQLServer2005;
            }
            else if (typeSystemVersionString.Equals(TYPESYSTEMVERSION.SQL_Server_2008, StringComparison.OrdinalIgnoreCase))
            {
                TypeSystemVersion = TypeSystem.SQLServer2008;
            }
            else if (typeSystemVersionString.Equals(TYPESYSTEMVERSION.SQL_Server_2012, StringComparison.OrdinalIgnoreCase))
            {
                TypeSystemVersion = TypeSystem.SQLServer2012;
                TypeSystemAssemblyVersion = constTypeSystemAsmVersion11;
            }
            else
            {
                throw ADP.InvalidConnectionOptionValue(KEY.Type_System_Version);
            }

            if (string.IsNullOrEmpty(transactionBindingString)) transactionBindingString = DbConnectionStringDefaults.TransactionBinding;

            if (transactionBindingString.Equals(TRANSACTIONBINDING.ImplicitUnbind, StringComparison.OrdinalIgnoreCase))
                TransactionBinding = TransactionBindingEnum.ImplicitUnbind;
            else if (transactionBindingString.Equals(TRANSACTIONBINDING.ExplicitUnbind, StringComparison.OrdinalIgnoreCase))
                TransactionBinding = TransactionBindingEnum.ExplicitUnbind;
            else
                throw ADP.InvalidConnectionOptionValue(KEY.TransactionBinding);

            if (ApplicationIntent == ApplicationIntent.ReadOnly && !string.IsNullOrEmpty(FailoverPartner))
                throw SQL.ROR_FailoverNotSupportedConnString();

            if (ConnectRetryCount < 0 || ConnectRetryCount > 255) throw ADP.InvalidConnectRetryCountValue();

            if (ConnectRetryInterval < 1 || ConnectRetryInterval > 60) throw ADP.InvalidConnectRetryIntervalValue();
        }


        // This c-tor is used to create SSE and user instance connection strings when user instance is set to true
        // BUG (VSTFDevDiv) 479687: Using TransactionScope with Linq2SQL against user instances fails with "connection has been broken" message
        internal SqlConnectionString(SqlConnectionString connectionOptions, string dataSource, bool userInstance, bool? setEnlistValue) : base(connectionOptions)
        {
            IntegratedSecurity = connectionOptions.IntegratedSecurity;
            Encrypt = connectionOptions.Encrypt;

            Enlist = setEnlistValue ?? connectionOptions.Enlist;

            MARS = connectionOptions.MARS;
            PersistSecurityInfo = connectionOptions.PersistSecurityInfo;
            Pooling = connectionOptions.Pooling;
            Replication = connectionOptions.Replication;
            UserInstance = userInstance;
            ConnectTimeout = connectionOptions.ConnectTimeout;
            LoadBalanceTimeout = connectionOptions.LoadBalanceTimeout;
            MaxPoolSize = connectionOptions.MaxPoolSize;
            MinPoolSize = connectionOptions.MinPoolSize;
            MultiSubnetFailover = connectionOptions.MultiSubnetFailover;
            PacketSize = connectionOptions.PacketSize;
            ApplicationName = connectionOptions.ApplicationName;
            AttachDBFilename = connectionOptions.AttachDBFilename;
            CurrentLanguage = connectionOptions.CurrentLanguage;
            DataSource = dataSource;
            //LocalDBInstance = LocalDbApi.GetLocalDbInstanceNameFromServerName(DataSource);
            FailoverPartner = connectionOptions.FailoverPartner;
            InitialCatalog = connectionOptions.InitialCatalog;
            Password = connectionOptions.Password;
            UserID = connectionOptions.UserID;
            WorkstationId = connectionOptions.WorkstationId;
            TypeSystemVersion = connectionOptions.TypeSystemVersion;
            TransactionBinding = connectionOptions.TransactionBinding;
            ApplicationIntent = connectionOptions.ApplicationIntent;
            ConnectRetryCount = connectionOptions.ConnectRetryCount;
            ConnectRetryInterval = connectionOptions.ConnectRetryInterval;

            ValidateValueLength(DataSource, TdsEnums.MAXLEN_SERVERNAME, KEY.Data_Source);
        }

        public string ConnectionString { get; set; }
        public bool IntegratedSecurity { get; }

        // We always initialize in Async mode so that both synchronous and asynchronous methods
        // will work.  In the future we can deprecate the keyword entirely.
        internal bool Asynchronous => true;

        // SQLPT 41700: Ignore ResetConnection=False, always reset the connection for security
        public bool ConnectionReset => true;

        //        internal bool EnableUdtDownload { get { return _enableUdtDownload;} }
        public bool Encrypt { get; }

        public bool TrustServerCertificate { get; }

        public bool Enlist { get; }

        public bool MARS { get; }

        public bool MultiSubnetFailover { get; }

        public bool PersistSecurityInfo { get; }

        public bool Pooling { get; }

        public bool Replication { get; }

        public bool UserInstance { get; }

        public int ConnectTimeout { get; }

        public int LoadBalanceTimeout { get; }

        public int MaxPoolSize { get; }

        public int MinPoolSize { get; }

        public int PacketSize { get; }

        public int ConnectRetryCount { get; }

        public int ConnectRetryInterval { get; }

        public ApplicationIntent ApplicationIntent { get; }

        public string ApplicationName { get; }

        public string AttachDBFilename { get; }

        public string CurrentLanguage { get; }

        public string DataSource { get; }

        public string LocalDBInstance { get; }

        public string FailoverPartner { get; }

        public string InitialCatalog { get; }

        public string Password { get; }

        public string UserID { get; }

        public string WorkstationId { get; }

        public TypeSystem TypeSystemVersion { get; }

        public Version TypeSystemAssemblyVersion { get; }

        public TransactionBindingEnum TransactionBinding { get; }

        // This dictionary is meant to be read-only translation of parsed string
        // keywords/synonyms to a known keyword string.
        internal static Dictionary<string, string> GetParseSynonyms()
        {
            var synonyms = s_sqlClientSynonyms;
            if (null != synonyms)
                return synonyms;
            const int count = SqlConnectionStringBuilder.KeywordsCount + SqlConnectionStringBuilder.DeprecatedKeywordsCount + SynonymCount + DeprecatedSynonymCount;
            synonyms = new Dictionary<string, string>(count)
            {
                {KEY.ApplicationIntent, KEY.ApplicationIntent},
                {KEY.Application_Name, KEY.Application_Name},
                {KEY.AsynchronousProcessing, KEY.AsynchronousProcessing},
                {KEY.AttachDBFilename, KEY.AttachDBFilename},
                {KEY.Connect_Timeout, KEY.Connect_Timeout},
                {KEY.Connection_Reset, KEY.Connection_Reset},
                {KEY.Context_Connection, KEY.Context_Connection},
                {KEY.Current_Language, KEY.Current_Language},
                {KEY.Data_Source, KEY.Data_Source},
                {KEY.Encrypt, KEY.Encrypt},
                {KEY.Enlist, KEY.Enlist},
                {KEY.FailoverPartner, KEY.FailoverPartner},
                {KEY.Initial_Catalog, KEY.Initial_Catalog},
                {KEY.Integrated_Security, KEY.Integrated_Security},
                {KEY.Load_Balance_Timeout, KEY.Load_Balance_Timeout},
                {KEY.MARS, KEY.MARS},
                {KEY.Max_Pool_Size, KEY.Max_Pool_Size},
                {KEY.Min_Pool_Size, KEY.Min_Pool_Size},
                {KEY.MultiSubnetFailover, KEY.MultiSubnetFailover},
                {KEY.Network_Library, KEY.Network_Library},
                {KEY.Packet_Size, KEY.Packet_Size},
                {KEY.Password, KEY.Password},
                {KEY.Persist_Security_Info, KEY.Persist_Security_Info},
                {KEY.Pooling, KEY.Pooling},
                {KEY.Replication, KEY.Replication},
                {KEY.TrustServerCertificate, KEY.TrustServerCertificate},
                {KEY.TransactionBinding, KEY.TransactionBinding},
                {KEY.Type_System_Version, KEY.Type_System_Version},
                {KEY.User_ID, KEY.User_ID},
                {KEY.User_Instance, KEY.User_Instance},
                {KEY.Workstation_Id, KEY.Workstation_Id},
                {KEY.Connect_Retry_Count, KEY.Connect_Retry_Count},
                {KEY.Connect_Retry_Interval, KEY.Connect_Retry_Interval},

                {SYNONYM.APP, KEY.Application_Name},
                {SYNONYM.Async, KEY.AsynchronousProcessing},
                {SYNONYM.EXTENDED_PROPERTIES, KEY.AttachDBFilename},
                {SYNONYM.INITIAL_FILE_NAME, KEY.AttachDBFilename},
                {SYNONYM.CONNECTION_TIMEOUT, KEY.Connect_Timeout},
                {SYNONYM.TIMEOUT, KEY.Connect_Timeout},
                {SYNONYM.LANGUAGE, KEY.Current_Language},
                {SYNONYM.ADDR, KEY.Data_Source},
                {SYNONYM.ADDRESS, KEY.Data_Source},
                {SYNONYM.NETWORK_ADDRESS, KEY.Data_Source},
                {SYNONYM.SERVER, KEY.Data_Source},
                {SYNONYM.DATABASE, KEY.Initial_Catalog},
                {SYNONYM.TRUSTED_CONNECTION, KEY.Integrated_Security},
                {SYNONYM.Connection_Lifetime, KEY.Load_Balance_Timeout},
                {SYNONYM.NET, KEY.Network_Library},
                {SYNONYM.NETWORK, KEY.Network_Library},
                {SYNONYM.Pwd, KEY.Password},
                {SYNONYM.PERSISTSECURITYINFO, KEY.Persist_Security_Info},
                {SYNONYM.UID, KEY.User_ID},
                {SYNONYM.User, KEY.User_ID},
                {SYNONYM.WSID, KEY.Workstation_Id}
            };
            Debug.Assert(synonyms.Count == count, "incorrect initial ParseSynonyms size");
            s_sqlClientSynonyms = synonyms;

            return synonyms;
        }

        public string ObtainWorkstationId()
        {
            // If not supplied by the user, the default value is the MachineName
            // Note: In Longhorn you'll be able to rename a machine without
            // rebooting.  Therefore, don't cache this machine name.
            var result = WorkstationId;
            if (null != result)
                return result;
            // permission to obtain Environment.MachineName is Asserted
            // since permission to open the connection has been granted
            // the information is shared with the server, but not directly with the user
            result = ADP.MachineName();
            ValidateValueLength(result, TdsEnums.MAXLEN_HOSTNAME, KEY.Workstation_Id);

            return result;
        }


        private static void ValidateValueLength(string value, int limit, string key)
        {
            if (limit < value.Length) throw ADP.InvalidConnectionOptionValueLength(key, limit);
        }


        internal ApplicationIntent ConvertValueToApplicationIntent()
        {
            if (!TryGetParsetableValue(KEY.ApplicationIntent, out var value)) return DEFAULT.ApplicationIntent;

            // when wrong value is used in the connection string provided to LoginProcessor.ConnectionString or c-tor,
            // wrap Format and Overflow exceptions with Argument one, to be consistent with rest of the keyword types (like int and bool)
            try
            {
                return DbConnectionStringBuilderUtil.ConvertToApplicationIntent(KEY.ApplicationIntent, value);
            }
            catch (FormatException e)
            {
                throw ADP.InvalidConnectionOptionValue(KEY.ApplicationIntent, e);
            }
            catch (OverflowException e)
            {
                throw ADP.InvalidConnectionOptionValue(KEY.ApplicationIntent, e);
            }

            // ArgumentException and other types are raised as is (no wrapping)
        }

        internal void ThrowUnsupportedIfKeywordSet(string keyword)
        {
            if (ContainsKey(keyword)) throw SQL.UnsupportedKeyword(keyword);
        }
        // instances of this class are intended to be immutable, i.e readonly
        // used by pooling classes so it is much easier to verify correctness
        // when not worried about the class being modified during execution

        internal static class DEFAULT
        {
            internal const ApplicationIntent ApplicationIntent = DbConnectionStringDefaults.ApplicationIntent;
            internal const string ApplicationName = TdsEnums.SQL_PROVIDER_NAME;
            internal const string AttachDbFilename = "";
            internal const int ConnectTimeout = ADP.DefaultConnectionTimeout;
            internal const string CurrentLanguage = "";
            internal const string DataSource = "";
            internal const bool Encrypt = false;
            internal const bool Enlist = true;
            internal const string FailoverPartner = "";
            internal const string InitialCatalog = "";
            internal const bool IntegratedSecurity = false;
            internal const int LoadBalanceTimeout = 0; // default of 0 means don't use
            internal const bool MARS = false;
            internal const int Max_Pool_Size = 100;
            internal const int Min_Pool_Size = 0;
            internal const bool MultiSubnetFailover = DbConnectionStringDefaults.MultiSubnetFailover;
            internal const int Packet_Size = 8000;
            internal const string Password = "";
            internal const bool Persist_Security_Info = false;
            internal const bool Pooling = true;
            internal const bool TrustServerCertificate = false;
            internal const string Type_System_Version = "";
            internal const string User_ID = "";
            internal const bool User_Instance = false;
            internal const bool Replication = false;
            internal const int Connect_Retry_Count = 1;
            internal const int Connect_Retry_Interval = 10;
        }

        // LoginProcessor ConnectionString Options
        // keys must be lowercase!
        internal static class KEY
        {
            internal const string ApplicationIntent = "applicationintent";
            internal const string Application_Name = "application name";
            internal const string AsynchronousProcessing = "asynchronous processing";
            internal const string AttachDBFilename = "attachdbfilename";
            internal const string Connect_Timeout = "connect timeout";
            internal const string Connection_Reset = "connection reset";
            internal const string Context_Connection = "context connection";
            internal const string Current_Language = "current language";
            internal const string Data_Source = "data source";
            internal const string Encrypt = "encrypt";
            internal const string Enlist = "enlist";
            internal const string FailoverPartner = "failover partner";
            internal const string Initial_Catalog = "initial catalog";
            internal const string Integrated_Security = "integrated security";
            internal const string Load_Balance_Timeout = "load balance timeout";
            internal const string MARS = "multipleactiveresultsets";
            internal const string Max_Pool_Size = "max pool size";
            internal const string Min_Pool_Size = "min pool size";
            internal const string MultiSubnetFailover = "multisubnetfailover";
            internal const string Network_Library = "network library";
            internal const string Packet_Size = "packet size";
            internal const string Password = "password";
            internal const string Persist_Security_Info = "persist security info";
            internal const string Pooling = "pooling";
            internal const string TransactionBinding = "transaction binding";
            internal const string TrustServerCertificate = "trustservercertificate";
            internal const string Type_System_Version = "type system version";
            internal const string User_ID = "user id";
            internal const string User_Instance = "user instance";
            internal const string Workstation_Id = "workstation id";
            internal const string Replication = "replication";
            internal const string Connect_Retry_Count = "connectretrycount";
            internal const string Connect_Retry_Interval = "connectretryinterval";
        }

        // Constant for the number of duplicate options in the connection string

        private static class SYNONYM
        {
            // application name
            internal const string APP = "app";

            internal const string Async = "async";

            // attachDBFilename
            internal const string EXTENDED_PROPERTIES = "extended properties";

            internal const string INITIAL_FILE_NAME = "initial file name";

            // connect timeout
            internal const string CONNECTION_TIMEOUT = "connection timeout";

            internal const string TIMEOUT = "timeout";

            // current language
            internal const string LANGUAGE = "language";

            // data source
            internal const string ADDR = "addr";
            internal const string ADDRESS = "address";
            internal const string SERVER = "server";

            internal const string NETWORK_ADDRESS = "network address";

            // initial catalog
            internal const string DATABASE = "database";

            // integrated security
            internal const string TRUSTED_CONNECTION = "trusted_connection";

            // load balance timeout
            internal const string Connection_Lifetime = "connection lifetime";

            // network library
            internal const string NET = "net";

            internal const string NETWORK = "network";

            // password
            internal const string Pwd = "pwd";

            // persist security info
            internal const string PERSISTSECURITYINFO = "persistsecurityinfo";

            // user id
            internal const string UID = "uid";

            internal const string User = "user";

            // workstation id
            internal const string WSID = "wsid";
            // make sure to update SynonymCount value below when adding or removing synonyms
        }

        public static class TYPESYSTEMVERSION
        {
            internal const string Latest = "Latest";
            internal const string SQL_Server_2000 = "SQL Server 2000";
            internal const string SQL_Server_2005 = "SQL Server 2005";
            internal const string SQL_Server_2008 = "SQL Server 2008";
            internal const string SQL_Server_2012 = "SQL Server 2012";
        }

        internal static class TRANSACTIONBINDING
        {
            internal const string ImplicitUnbind = "Implicit Unbind";
            internal const string ExplicitUnbind = "Explicit Unbind";
        }
    }
}