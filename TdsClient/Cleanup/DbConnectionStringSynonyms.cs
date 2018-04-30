namespace Medella.TdsClient.Cleanup
{
    internal static class DbConnectionStringSynonyms
    {
        //internal const string AsynchronousProcessing = Async;
        internal const string Async = "async";

        //internal const string ApplicationName        = APP;
        internal const string APP = "app";

        //internal const string AttachDBFilename       = EXTENDEDPROPERTIES+","+INITIALFILENAME;
        internal const string EXTENDEDPROPERTIES = "extended properties";
        internal const string INITIALFILENAME = "initial file name";

        //internal const string ConnectTimeout         = CONNECTIONTIMEOUT+","+TIMEOUT;
        internal const string CONNECTIONTIMEOUT = "connection timeout";
        internal const string TIMEOUT = "timeout";

        //internal const string CurrentLanguage        = LANGUAGE;
        internal const string LANGUAGE = "language";

        //internal const string OraDataSource          = SERVER;
        //internal const string SqlDataSource          = ADDR+","+ADDRESS+","+SERVER+","+NETWORKADDRESS;
        internal const string ADDR = "addr";
        internal const string ADDRESS = "address";
        internal const string SERVER = "server";
        internal const string NETWORKADDRESS = "network address";

        //internal const string InitialCatalog         = DATABASE;
        internal const string DATABASE = "database";

        //internal const string IntegratedSecurity     = TRUSTEDCONNECTION;
        internal const string TRUSTEDCONNECTION = "trusted_connection"; // underscore introduced in Everett

        //internal const string LoadBalanceTimeout     = ConnectionLifetime;
        internal const string ConnectionLifetime = "connection lifetime";

        //internal const string NetworkLibrary         = NET+","+NETWORK;
        internal const string NET = "net";
        internal const string NETWORK = "network";


        //internal const string Password               = Pwd;
        internal const string Pwd = "pwd";

        //internal const string PersistSecurityInfo    = PERSISTSECURITYINFO;
        internal const string PERSISTSECURITYINFO = "persistsecurityinfo";

        //internal const string UserID                 = UID+","+User;
        internal const string UID = "uid";
        internal const string User = "user";

        //internal const string WorkstationID          = WSID;
        internal const string WSID = "wsid";
    }
}