using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Medella.TdsClient.Contants;
using Medella.TdsClient.Exceptions;

namespace Medella.TdsClient.Cleanup
{
    public sealed class SqlConnectionStringBuilder : DbConnectionStringBuilder
    {
        internal const int KeywordsCount = (int) Keywords.KeywordsCount;
        internal const int DeprecatedKeywordsCount = 4;

        private static readonly string[] SValidKeywords = CreateValidKeywords();
        private static readonly Dictionary<string, Keywords> SKeywords = CreateKeywordsDictionary();

        private static readonly string[] SNotSupportedKeywords =
        {
            DbConnectionStringKeywords.AsynchronousProcessing,
            DbConnectionStringKeywords.ConnectionReset,
            DbConnectionStringKeywords.ContextConnection,
            DbConnectionStringKeywords.TransactionBinding,

            DbConnectionStringSynonyms.Async
        };

        private static readonly string[] SNotSupportedNetworkLibraryKeywords =
        {
            DbConnectionStringKeywords.NetworkLibrary,
            DbConnectionStringSynonyms.NET,
            DbConnectionStringSynonyms.NETWORK
        };

        private ApplicationIntent _applicationIntent = DbConnectionStringDefaults.ApplicationIntent;
        private string _applicationName = DbConnectionStringDefaults.ApplicationName;
        private string _attachDbFilename = DbConnectionStringDefaults.AttachDBFilename;
        private int _connectRetryCount = DbConnectionStringDefaults.ConnectRetryCount;
        private int _connectRetryInterval = DbConnectionStringDefaults.ConnectRetryInterval;

        private int _connectTimeout = DbConnectionStringDefaults.ConnectTimeout;
        private string _currentLanguage = DbConnectionStringDefaults.CurrentLanguage;
        private string _dataSource = DbConnectionStringDefaults.DataSource;

        private bool _encrypt = DbConnectionStringDefaults.Encrypt;
        private bool _enlist = DbConnectionStringDefaults.Enlist;
        private string _failoverPartner = DbConnectionStringDefaults.FailoverPartner;
        private string _initialCatalog = DbConnectionStringDefaults.InitialCatalog;
        private bool _integratedSecurity = DbConnectionStringDefaults.IntegratedSecurity;
        private int _loadBalanceTimeout = DbConnectionStringDefaults.LoadBalanceTimeout;
        private int _maxPoolSize = DbConnectionStringDefaults.MaxPoolSize;
        private int _minPoolSize = DbConnectionStringDefaults.MinPoolSize;
        private bool _multipleActiveResultSets = DbConnectionStringDefaults.MultipleActiveResultSets;
        private bool _multiSubnetFailover = DbConnectionStringDefaults.MultiSubnetFailover;

        private int _packetSize = DbConnectionStringDefaults.PacketSize;

        private string _password = DbConnectionStringDefaults.Password;
        private bool _persistSecurityInfo = DbConnectionStringDefaults.PersistSecurityInfo;
        private bool _pooling = DbConnectionStringDefaults.Pooling;
        private bool _replication = DbConnectionStringDefaults.Replication;
        private string _transactionBinding = DbConnectionStringDefaults.TransactionBinding;
        private bool _trustServerCertificate = DbConnectionStringDefaults.TrustServerCertificate;
        private string _typeSystemVersion = DbConnectionStringDefaults.TypeSystemVersion;
        private string _userId = DbConnectionStringDefaults.UserID;
        private bool _userInstance = DbConnectionStringDefaults.UserInstance;
        private string _workstationId = DbConnectionStringDefaults.WorkstationID;

        public SqlConnectionStringBuilder() : this(null)
        {
        }

        public SqlConnectionStringBuilder(string connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString)) ConnectionString = connectionString;
        }

        public override object this[string keyword]
        {
            get
            {
                var index = GetIndex(keyword);
                return GetAt(index);
            }
            set
            {
                if (null != value)
                {
                    var index = GetIndex(keyword);
                    switch (index)
                    {
                        case Keywords.ApplicationIntent:
                            ApplicationIntent = ConvertToApplicationIntent(keyword, value);
                            break;
                        case Keywords.ApplicationName:
                            ApplicationName = ConvertToString(value);
                            break;
                        case Keywords.AttachDBFilename:
                            AttachDBFilename = ConvertToString(value);
                            break;
                        case Keywords.CurrentLanguage:
                            CurrentLanguage = ConvertToString(value);
                            break;
                        case Keywords.DataSource:
                            DataSource = ConvertToString(value);
                            break;
                        case Keywords.FailoverPartner:
                            FailoverPartner = ConvertToString(value);
                            break;
                        case Keywords.InitialCatalog:
                            InitialCatalog = ConvertToString(value);
                            break;
                        //                  case Keywords.NamedConnection:          NamedConnection = ConvertToString(value); break;
                        case Keywords.Password:
                            Password = ConvertToString(value);
                            break;
                        case Keywords.UserID:
                            UserID = ConvertToString(value);
                            break;
                        case Keywords.TransactionBinding:
                            TransactionBinding = ConvertToString(value);
                            break;
                        case Keywords.TypeSystemVersion:
                            TypeSystemVersion = ConvertToString(value);
                            break;
                        case Keywords.WorkstationID:
                            WorkstationID = ConvertToString(value);
                            break;

                        case Keywords.ConnectTimeout:
                            ConnectTimeout = ConvertToInt32(value);
                            break;
                        case Keywords.LoadBalanceTimeout:
                            LoadBalanceTimeout = ConvertToInt32(value);
                            break;
                        case Keywords.MaxPoolSize:
                            MaxPoolSize = ConvertToInt32(value);
                            break;
                        case Keywords.MinPoolSize:
                            MinPoolSize = ConvertToInt32(value);
                            break;
                        case Keywords.PacketSize:
                            PacketSize = ConvertToInt32(value);
                            break;

                        case Keywords.IntegratedSecurity:
                            IntegratedSecurity = ConvertToIntegratedSecurity(value);
                            break;

                        case Keywords.Encrypt:
                            Encrypt = ConvertToBoolean(value);
                            break;
                        case Keywords.TrustServerCertificate:
                            TrustServerCertificate = ConvertToBoolean(value);
                            break;
                        case Keywords.Enlist:
                            Enlist = ConvertToBoolean(value);
                            break;
                        case Keywords.MultipleActiveResultSets:
                            MultipleActiveResultSets = ConvertToBoolean(value);
                            break;
                        case Keywords.MultiSubnetFailover:
                            MultiSubnetFailover = ConvertToBoolean(value);
                            break;
                        case Keywords.PersistSecurityInfo:
                            PersistSecurityInfo = ConvertToBoolean(value);
                            break;
                        case Keywords.Pooling:
                            Pooling = ConvertToBoolean(value);
                            break;
                        case Keywords.Replication:
                            Replication = ConvertToBoolean(value);
                            break;
                        case Keywords.UserInstance:
                            UserInstance = ConvertToBoolean(value);
                            break;
                        case Keywords.ConnectRetryCount:
                            ConnectRetryCount = ConvertToInt32(value);
                            break;
                        case Keywords.ConnectRetryInterval:
                            ConnectRetryInterval = ConvertToInt32(value);
                            break;

                        default:
                            Debug.Assert(false, "unexpected keyword");
                            throw UnsupportedKeyword(keyword);
                    }
                }
                else
                {
                    Remove(keyword);
                }
            }
        }

        public ApplicationIntent ApplicationIntent
        {
            get => _applicationIntent;
            set
            {
                if (!DbConnectionStringBuilderUtil.IsValidApplicationIntentValue(value)) throw ADP.InvalidEnumerationValue(typeof(ApplicationIntent), (int) value);

                SetApplicationIntentValue(value);
                _applicationIntent = value;
            }
        }

        public string ApplicationName
        {
            get => _applicationName;
            set
            {
                SetValue(DbConnectionStringKeywords.ApplicationName, value);
                _applicationName = value;
            }
        }

        public string AttachDBFilename
        {
            get => _attachDbFilename;
            set
            {
                SetValue(DbConnectionStringKeywords.AttachDBFilename, value);
                _attachDbFilename = value;
            }
        }

        public int ConnectTimeout
        {
            get => _connectTimeout;
            set
            {
                if (value < 0) throw ADP.InvalidConnectionOptionValue(DbConnectionStringKeywords.ConnectTimeout);
                SetValue(DbConnectionStringKeywords.ConnectTimeout, value);
                _connectTimeout = value;
            }
        }

        public string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                SetValue(DbConnectionStringKeywords.CurrentLanguage, value);
                _currentLanguage = value;
            }
        }

        public string DataSource
        {
            get => _dataSource;
            set
            {
                SetValue(DbConnectionStringKeywords.DataSource, value);
                _dataSource = value;
            }
        }

        public bool Encrypt
        {
            get => _encrypt;
            set
            {
                SetValue(DbConnectionStringKeywords.Encrypt, value);
                _encrypt = value;
            }
        }

        public bool TrustServerCertificate
        {
            get => _trustServerCertificate;
            set
            {
                SetValue(DbConnectionStringKeywords.TrustServerCertificate, value);
                _trustServerCertificate = value;
            }
        }

        public bool Enlist
        {
            get => _enlist;
            set
            {
                SetValue(DbConnectionStringKeywords.Enlist, value);
                _enlist = value;
            }
        }

        public string FailoverPartner
        {
            get => _failoverPartner;
            set
            {
                SetValue(DbConnectionStringKeywords.FailoverPartner, value);
                _failoverPartner = value;
            }
        }

        [TypeConverter(typeof(SqlInitialCatalogConverter))]
        public string InitialCatalog
        {
            get => _initialCatalog;
            set
            {
                SetValue(DbConnectionStringKeywords.InitialCatalog, value);
                _initialCatalog = value;
            }
        }

        public bool IntegratedSecurity
        {
            get => _integratedSecurity;
            set
            {
                SetValue(DbConnectionStringKeywords.IntegratedSecurity, value);
                _integratedSecurity = value;
            }
        }

        public int LoadBalanceTimeout
        {
            get => _loadBalanceTimeout;
            set
            {
                if (value < 0) throw ADP.InvalidConnectionOptionValue(DbConnectionStringKeywords.LoadBalanceTimeout);
                SetValue(DbConnectionStringKeywords.LoadBalanceTimeout, value);
                _loadBalanceTimeout = value;
            }
        }

        public int MaxPoolSize
        {
            get => _maxPoolSize;
            set
            {
                if (value < 1) throw ADP.InvalidConnectionOptionValue(DbConnectionStringKeywords.MaxPoolSize);
                SetValue(DbConnectionStringKeywords.MaxPoolSize, value);
                _maxPoolSize = value;
            }
        }

        public int ConnectRetryCount
        {
            get => _connectRetryCount;
            set
            {
                if (value < 0 || value > 255) throw ADP.InvalidConnectionOptionValue(DbConnectionStringKeywords.ConnectRetryCount);
                SetValue(DbConnectionStringKeywords.ConnectRetryCount, value);
                _connectRetryCount = value;
            }
        }

        public int ConnectRetryInterval
        {
            get => _connectRetryInterval;
            set
            {
                if (value < 1 || value > 60) throw ADP.InvalidConnectionOptionValue(DbConnectionStringKeywords.ConnectRetryInterval);
                SetValue(DbConnectionStringKeywords.ConnectRetryInterval, value);
                _connectRetryInterval = value;
            }
        }


        public int MinPoolSize
        {
            get => _minPoolSize;
            set
            {
                if (value < 0) throw ADP.InvalidConnectionOptionValue(DbConnectionStringKeywords.MinPoolSize);
                SetValue(DbConnectionStringKeywords.MinPoolSize, value);
                _minPoolSize = value;
            }
        }

        public bool MultipleActiveResultSets
        {
            get => _multipleActiveResultSets;
            set
            {
                SetValue(DbConnectionStringKeywords.MultipleActiveResultSets, value);
                _multipleActiveResultSets = value;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Reviewed and Approved by UE")]
        public bool MultiSubnetFailover
        {
            get => _multiSubnetFailover;
            set
            {
                SetValue(DbConnectionStringKeywords.MultiSubnetFailover, value);
                _multiSubnetFailover = value;
            }
        }

        /*
                [DisplayName(DbConnectionStringKeywords.NamedConnection)]
                [ResCategoryAttribute(Res.DataCategory_NamedConnectionString)]
                [ResDescriptionAttribute(Res.DbConnectionString_NamedConnection)]
                [RefreshPropertiesAttribute(RefreshProperties.All)]
                [TypeConverter(typeof(NamedConnectionStringConverter))]
                public string NamedConnection {
                    get { return _namedConnection; }
                    set {
                        SetValue(DbConnectionStringKeywords.NamedConnection, value);
                        _namedConnection = value;
                    }
                }
        */
        public int PacketSize
        {
            get => _packetSize;
            set
            {
                if (value < TdsEnums.MIN_PACKET_SIZE || TdsEnums.MAX_PACKET_SIZE < value) throw SQL.InvalidPacketSizeValue();
                SetValue(DbConnectionStringKeywords.PacketSize, value);
                _packetSize = value;
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetValue(DbConnectionStringKeywords.Password, value);
                _password = value;
            }
        }

        public bool PersistSecurityInfo
        {
            get => _persistSecurityInfo;
            set
            {
                SetValue(DbConnectionStringKeywords.PersistSecurityInfo, value);
                _persistSecurityInfo = value;
            }
        }

        public bool Pooling
        {
            get => _pooling;
            set
            {
                SetValue(DbConnectionStringKeywords.Pooling, value);
                _pooling = value;
            }
        }

        public bool Replication
        {
            get => _replication;
            set
            {
                SetValue(DbConnectionStringKeywords.Replication, value);
                _replication = value;
            }
        }

        public string TransactionBinding
        {
            get => _transactionBinding;
            set
            {
                SetValue(DbConnectionStringKeywords.TransactionBinding, value);
                _transactionBinding = value;
            }
        }

        public string TypeSystemVersion
        {
            get => _typeSystemVersion;
            set
            {
                SetValue(DbConnectionStringKeywords.TypeSystemVersion, value);
                _typeSystemVersion = value;
            }
        }

        public string UserID
        {
            get => _userId;
            set
            {
                SetValue(DbConnectionStringKeywords.UserID, value);
                _userId = value;
            }
        }

        public bool UserInstance
        {
            get => _userInstance;
            set
            {
                SetValue(DbConnectionStringKeywords.UserInstance, value);
                _userInstance = value;
            }
        }

        public string WorkstationID
        {
            get => _workstationId;
            set
            {
                SetValue(DbConnectionStringKeywords.WorkstationID, value);
                _workstationId = value;
            }
        }


        public override ICollection Keys => new ReadOnlyCollection<string>(SValidKeywords);

        public override ICollection Values
        {
            get
            {
                // written this way so if the ordering of Keywords & _validKeywords changes
                // this is one less place to maintain
                var values = new object[SValidKeywords.Length];
                for (var i = 0; i < values.Length; ++i) values[i] = GetAt((Keywords) i);
                return new ReadOnlyCollection<object>(values);
            }
        }

        private static string[] CreateValidKeywords()
        {
            var validKeywords = new string[KeywordsCount];
            validKeywords[(int) Keywords.ApplicationIntent] = DbConnectionStringKeywords.ApplicationIntent;
            validKeywords[(int) Keywords.ApplicationName] = DbConnectionStringKeywords.ApplicationName;
            validKeywords[(int) Keywords.AttachDBFilename] = DbConnectionStringKeywords.AttachDBFilename;
            validKeywords[(int) Keywords.ConnectTimeout] = DbConnectionStringKeywords.ConnectTimeout;
            validKeywords[(int) Keywords.CurrentLanguage] = DbConnectionStringKeywords.CurrentLanguage;
            validKeywords[(int) Keywords.DataSource] = DbConnectionStringKeywords.DataSource;
            validKeywords[(int) Keywords.Encrypt] = DbConnectionStringKeywords.Encrypt;
            validKeywords[(int) Keywords.Enlist] = DbConnectionStringKeywords.Enlist;
            validKeywords[(int) Keywords.FailoverPartner] = DbConnectionStringKeywords.FailoverPartner;
            validKeywords[(int) Keywords.InitialCatalog] = DbConnectionStringKeywords.InitialCatalog;
            validKeywords[(int) Keywords.IntegratedSecurity] = DbConnectionStringKeywords.IntegratedSecurity;
            validKeywords[(int) Keywords.LoadBalanceTimeout] = DbConnectionStringKeywords.LoadBalanceTimeout;
            validKeywords[(int) Keywords.MaxPoolSize] = DbConnectionStringKeywords.MaxPoolSize;
            validKeywords[(int) Keywords.MinPoolSize] = DbConnectionStringKeywords.MinPoolSize;
            validKeywords[(int) Keywords.MultipleActiveResultSets] = DbConnectionStringKeywords.MultipleActiveResultSets;
            validKeywords[(int) Keywords.MultiSubnetFailover] = DbConnectionStringKeywords.MultiSubnetFailover;
            //          validKeywords[(int)Keywords.NamedConnection]          = DbConnectionStringKeywords.NamedConnection;
            validKeywords[(int) Keywords.PacketSize] = DbConnectionStringKeywords.PacketSize;
            validKeywords[(int) Keywords.Password] = DbConnectionStringKeywords.Password;
            validKeywords[(int) Keywords.PersistSecurityInfo] = DbConnectionStringKeywords.PersistSecurityInfo;
            validKeywords[(int) Keywords.Pooling] = DbConnectionStringKeywords.Pooling;
            validKeywords[(int) Keywords.Replication] = DbConnectionStringKeywords.Replication;
            validKeywords[(int) Keywords.TransactionBinding] = DbConnectionStringKeywords.TransactionBinding;
            validKeywords[(int) Keywords.TrustServerCertificate] = DbConnectionStringKeywords.TrustServerCertificate;
            validKeywords[(int) Keywords.TypeSystemVersion] = DbConnectionStringKeywords.TypeSystemVersion;
            validKeywords[(int) Keywords.UserID] = DbConnectionStringKeywords.UserID;
            validKeywords[(int) Keywords.UserInstance] = DbConnectionStringKeywords.UserInstance;
            validKeywords[(int) Keywords.WorkstationID] = DbConnectionStringKeywords.WorkstationID;
            validKeywords[(int) Keywords.ConnectRetryCount] = DbConnectionStringKeywords.ConnectRetryCount;
            validKeywords[(int) Keywords.ConnectRetryInterval] = DbConnectionStringKeywords.ConnectRetryInterval;
            return validKeywords;
        }

        private static Dictionary<string, Keywords> CreateKeywordsDictionary()
        {
            var hash = new Dictionary<string, Keywords>(KeywordsCount + SqlConnectionString.SynonymCount, StringComparer.OrdinalIgnoreCase);
            hash.Add(DbConnectionStringKeywords.ApplicationIntent, Keywords.ApplicationIntent);
            hash.Add(DbConnectionStringKeywords.ApplicationName, Keywords.ApplicationName);
            hash.Add(DbConnectionStringKeywords.AttachDBFilename, Keywords.AttachDBFilename);
            hash.Add(DbConnectionStringKeywords.ConnectTimeout, Keywords.ConnectTimeout);
            hash.Add(DbConnectionStringKeywords.CurrentLanguage, Keywords.CurrentLanguage);
            hash.Add(DbConnectionStringKeywords.DataSource, Keywords.DataSource);
            hash.Add(DbConnectionStringKeywords.Encrypt, Keywords.Encrypt);
            hash.Add(DbConnectionStringKeywords.Enlist, Keywords.Enlist);
            hash.Add(DbConnectionStringKeywords.FailoverPartner, Keywords.FailoverPartner);
            hash.Add(DbConnectionStringKeywords.InitialCatalog, Keywords.InitialCatalog);
            hash.Add(DbConnectionStringKeywords.IntegratedSecurity, Keywords.IntegratedSecurity);
            hash.Add(DbConnectionStringKeywords.LoadBalanceTimeout, Keywords.LoadBalanceTimeout);
            hash.Add(DbConnectionStringKeywords.MultipleActiveResultSets, Keywords.MultipleActiveResultSets);
            hash.Add(DbConnectionStringKeywords.MaxPoolSize, Keywords.MaxPoolSize);
            hash.Add(DbConnectionStringKeywords.MinPoolSize, Keywords.MinPoolSize);
            hash.Add(DbConnectionStringKeywords.MultiSubnetFailover, Keywords.MultiSubnetFailover);
            //          hash.Add(DbConnectionStringKeywords.NamedConnection,          Keywords.NamedConnection);
            hash.Add(DbConnectionStringKeywords.PacketSize, Keywords.PacketSize);
            hash.Add(DbConnectionStringKeywords.Password, Keywords.Password);
            hash.Add(DbConnectionStringKeywords.PersistSecurityInfo, Keywords.PersistSecurityInfo);
            hash.Add(DbConnectionStringKeywords.Pooling, Keywords.Pooling);
            hash.Add(DbConnectionStringKeywords.Replication, Keywords.Replication);
            hash.Add(DbConnectionStringKeywords.TransactionBinding, Keywords.TransactionBinding);
            hash.Add(DbConnectionStringKeywords.TrustServerCertificate, Keywords.TrustServerCertificate);
            hash.Add(DbConnectionStringKeywords.TypeSystemVersion, Keywords.TypeSystemVersion);
            hash.Add(DbConnectionStringKeywords.UserID, Keywords.UserID);
            hash.Add(DbConnectionStringKeywords.UserInstance, Keywords.UserInstance);
            hash.Add(DbConnectionStringKeywords.WorkstationID, Keywords.WorkstationID);
            hash.Add(DbConnectionStringKeywords.ConnectRetryCount, Keywords.ConnectRetryCount);
            hash.Add(DbConnectionStringKeywords.ConnectRetryInterval, Keywords.ConnectRetryInterval);

            hash.Add(DbConnectionStringSynonyms.APP, Keywords.ApplicationName);
            hash.Add(DbConnectionStringSynonyms.EXTENDEDPROPERTIES, Keywords.AttachDBFilename);
            hash.Add(DbConnectionStringSynonyms.INITIALFILENAME, Keywords.AttachDBFilename);
            hash.Add(DbConnectionStringSynonyms.CONNECTIONTIMEOUT, Keywords.ConnectTimeout);
            hash.Add(DbConnectionStringSynonyms.TIMEOUT, Keywords.ConnectTimeout);
            hash.Add(DbConnectionStringSynonyms.LANGUAGE, Keywords.CurrentLanguage);
            hash.Add(DbConnectionStringSynonyms.ADDR, Keywords.DataSource);
            hash.Add(DbConnectionStringSynonyms.ADDRESS, Keywords.DataSource);
            hash.Add(DbConnectionStringSynonyms.NETWORKADDRESS, Keywords.DataSource);
            hash.Add(DbConnectionStringSynonyms.SERVER, Keywords.DataSource);
            hash.Add(DbConnectionStringSynonyms.DATABASE, Keywords.InitialCatalog);
            hash.Add(DbConnectionStringSynonyms.TRUSTEDCONNECTION, Keywords.IntegratedSecurity);
            hash.Add(DbConnectionStringSynonyms.ConnectionLifetime, Keywords.LoadBalanceTimeout);
            hash.Add(DbConnectionStringSynonyms.Pwd, Keywords.Password);
            hash.Add(DbConnectionStringSynonyms.PERSISTSECURITYINFO, Keywords.PersistSecurityInfo);
            hash.Add(DbConnectionStringSynonyms.UID, Keywords.UserID);
            hash.Add(DbConnectionStringSynonyms.User, Keywords.UserID);
            hash.Add(DbConnectionStringSynonyms.WSID, Keywords.WorkstationID);
            Debug.Assert(KeywordsCount + SqlConnectionString.SynonymCount == hash.Count, "initial expected size is incorrect");
            return hash;
        }

        public override void Clear()
        {
            base.Clear();
            for (var i = 0; i < SValidKeywords.Length; ++i) Reset((Keywords) i);
        }

        public override bool ContainsKey(string keyword)
        {
            ADP.CheckArgumentNull(keyword, nameof(keyword));
            return SKeywords.ContainsKey(keyword);
        }

        private static bool ConvertToBoolean(object value)
        {
            return DbConnectionStringBuilderUtil.ConvertToBoolean(value);
        }

        private static int ConvertToInt32(object value)
        {
            return DbConnectionStringBuilderUtil.ConvertToInt32(value);
        }

        private static bool ConvertToIntegratedSecurity(object value)
        {
            return DbConnectionStringBuilderUtil.ConvertToIntegratedSecurity(value);
        }

        private static string ConvertToString(object value)
        {
            return DbConnectionStringBuilderUtil.ConvertToString(value);
        }

        private static ApplicationIntent ConvertToApplicationIntent(string keyword, object value)
        {
            return DbConnectionStringBuilderUtil.ConvertToApplicationIntent(keyword, value);
        }

        private object GetAt(Keywords index)
        {
            switch (index)
            {
                case Keywords.ApplicationIntent: return ApplicationIntent;
                case Keywords.ApplicationName: return ApplicationName;
                case Keywords.AttachDBFilename: return AttachDBFilename;
                case Keywords.ConnectTimeout: return ConnectTimeout;
                case Keywords.CurrentLanguage: return CurrentLanguage;
                case Keywords.DataSource: return DataSource;
                case Keywords.Encrypt: return Encrypt;
                case Keywords.Enlist: return Enlist;
                case Keywords.FailoverPartner: return FailoverPartner;
                case Keywords.InitialCatalog: return InitialCatalog;
                case Keywords.IntegratedSecurity: return IntegratedSecurity;
                case Keywords.LoadBalanceTimeout: return LoadBalanceTimeout;
                case Keywords.MultipleActiveResultSets: return MultipleActiveResultSets;
                case Keywords.MaxPoolSize: return MaxPoolSize;
                case Keywords.MinPoolSize: return MinPoolSize;
                case Keywords.MultiSubnetFailover: return MultiSubnetFailover;
                //          case Keywords.NamedConnection:          return NamedConnection;
                case Keywords.PacketSize: return PacketSize;
                case Keywords.Password: return Password;
                case Keywords.PersistSecurityInfo: return PersistSecurityInfo;
                case Keywords.Pooling: return Pooling;
                case Keywords.Replication: return Replication;
                case Keywords.TransactionBinding: return TransactionBinding;
                case Keywords.TrustServerCertificate: return TrustServerCertificate;
                case Keywords.TypeSystemVersion: return TypeSystemVersion;
                case Keywords.UserID: return UserID;
                case Keywords.UserInstance: return UserInstance;
                case Keywords.WorkstationID: return WorkstationID;
                case Keywords.ConnectRetryCount: return ConnectRetryCount;
                case Keywords.ConnectRetryInterval: return ConnectRetryInterval;

                default:
                    Debug.Assert(false, "unexpected keyword");
                    throw UnsupportedKeyword(SValidKeywords[(int) index]);
            }
        }

        private Keywords GetIndex(string keyword)
        {
            ADP.CheckArgumentNull(keyword, nameof(keyword));
            if (SKeywords.TryGetValue(keyword, out var index)) return index;
            throw UnsupportedKeyword(keyword);
        }


        public override bool Remove(string keyword)
        {
            ADP.CheckArgumentNull(keyword, nameof(keyword));
            if (SKeywords.TryGetValue(keyword, out var index))
                if (base.Remove(SValidKeywords[(int) index]))
                {
                    Reset(index);
                    return true;
                }

            return false;
        }

        private void Reset(Keywords index)
        {
            switch (index)
            {
                case Keywords.ApplicationIntent:
                    _applicationIntent = DbConnectionStringDefaults.ApplicationIntent;
                    break;
                case Keywords.ApplicationName:
                    _applicationName = DbConnectionStringDefaults.ApplicationName;
                    break;
                case Keywords.AttachDBFilename:
                    _attachDbFilename = DbConnectionStringDefaults.AttachDBFilename;
                    break;
                case Keywords.ConnectTimeout:
                    _connectTimeout = DbConnectionStringDefaults.ConnectTimeout;
                    break;
                case Keywords.CurrentLanguage:
                    _currentLanguage = DbConnectionStringDefaults.CurrentLanguage;
                    break;
                case Keywords.DataSource:
                    _dataSource = DbConnectionStringDefaults.DataSource;
                    break;
                case Keywords.Encrypt:
                    _encrypt = DbConnectionStringDefaults.Encrypt;
                    break;
                case Keywords.Enlist:
                    _enlist = DbConnectionStringDefaults.Enlist;
                    break;
                case Keywords.FailoverPartner:
                    _failoverPartner = DbConnectionStringDefaults.FailoverPartner;
                    break;
                case Keywords.InitialCatalog:
                    _initialCatalog = DbConnectionStringDefaults.InitialCatalog;
                    break;
                case Keywords.IntegratedSecurity:
                    _integratedSecurity = DbConnectionStringDefaults.IntegratedSecurity;
                    break;
                case Keywords.LoadBalanceTimeout:
                    _loadBalanceTimeout = DbConnectionStringDefaults.LoadBalanceTimeout;
                    break;
                case Keywords.MultipleActiveResultSets:
                    _multipleActiveResultSets = DbConnectionStringDefaults.MultipleActiveResultSets;
                    break;
                case Keywords.MaxPoolSize:
                    _maxPoolSize = DbConnectionStringDefaults.MaxPoolSize;
                    break;
                case Keywords.MinPoolSize:
                    _minPoolSize = DbConnectionStringDefaults.MinPoolSize;
                    break;
                case Keywords.MultiSubnetFailover:
                    _multiSubnetFailover = DbConnectionStringDefaults.MultiSubnetFailover;
                    break;
                //          case Keywords.NamedConnection:
                //              _namedConnection = DbConnectionStringDefaults.NamedConnection;
                //              break;
                case Keywords.PacketSize:
                    _packetSize = DbConnectionStringDefaults.PacketSize;
                    break;
                case Keywords.Password:
                    _password = DbConnectionStringDefaults.Password;
                    break;
                case Keywords.PersistSecurityInfo:
                    _persistSecurityInfo = DbConnectionStringDefaults.PersistSecurityInfo;
                    break;
                case Keywords.Pooling:
                    _pooling = DbConnectionStringDefaults.Pooling;
                    break;
                case Keywords.ConnectRetryCount:
                    _connectRetryCount = DbConnectionStringDefaults.ConnectRetryCount;
                    break;
                case Keywords.ConnectRetryInterval:
                    _connectRetryInterval = DbConnectionStringDefaults.ConnectRetryInterval;
                    break;
                case Keywords.Replication:
                    _replication = DbConnectionStringDefaults.Replication;
                    break;
                case Keywords.TransactionBinding:
                    _transactionBinding = DbConnectionStringDefaults.TransactionBinding;
                    break;
                case Keywords.TrustServerCertificate:
                    _trustServerCertificate = DbConnectionStringDefaults.TrustServerCertificate;
                    break;
                case Keywords.TypeSystemVersion:
                    _typeSystemVersion = DbConnectionStringDefaults.TypeSystemVersion;
                    break;
                case Keywords.UserID:
                    _userId = DbConnectionStringDefaults.UserID;
                    break;
                case Keywords.UserInstance:
                    _userInstance = DbConnectionStringDefaults.UserInstance;
                    break;
                case Keywords.WorkstationID:
                    _workstationId = DbConnectionStringDefaults.WorkstationID;
                    break;
                default:
                    Debug.Assert(false, "unexpected keyword");
                    throw UnsupportedKeyword(SValidKeywords[(int) index]);
            }
        }

        private void SetValue(string keyword, bool value)
        {
            base[keyword] = value.ToString();
        }

        private void SetValue(string keyword, int value)
        {
            base[keyword] = value.ToString((IFormatProvider) null);
        }

        private void SetValue(string keyword, string value)
        {
            ADP.CheckArgumentNull(value, keyword);
            base[keyword] = value;
        }

        private void SetApplicationIntentValue(ApplicationIntent value)
        {
            Debug.Assert(DbConnectionStringBuilderUtil.IsValidApplicationIntentValue(value), "invalid value");
            base[DbConnectionStringKeywords.ApplicationIntent] = DbConnectionStringBuilderUtil.ApplicationIntentToString(value);
        }

        public override bool ShouldSerialize(string keyword)
        {
            ADP.CheckArgumentNull(keyword, nameof(keyword));
            return SKeywords.TryGetValue(keyword, out var index) && base.ShouldSerialize(SValidKeywords[(int) index]);
        }

        public override bool TryGetValue(string keyword, out object value)
        {
            if (SKeywords.TryGetValue(keyword, out var index))
            {
                value = GetAt(index);
                return true;
            }

            value = null;
            return false;
        }

        private Exception UnsupportedKeyword(string keyword)
        {
            if (SNotSupportedKeywords.Contains(keyword, StringComparer.OrdinalIgnoreCase))
                return SQL.UnsupportedKeyword(keyword);
            if (SNotSupportedNetworkLibraryKeywords.Contains(keyword, StringComparer.OrdinalIgnoreCase))
                return SQL.NetworkLibraryKeywordNotSupported();
            return ADP.KeywordNotSupported(keyword);
        }

        private enum Keywords
        {
            // specific ordering for ConnectionString output construction
            // NamedConnection,
            DataSource,
            FailoverPartner,
            AttachDBFilename,
            InitialCatalog,
            IntegratedSecurity,
            PersistSecurityInfo,
            UserID,
            Password,

            Enlist,
            Pooling,
            MinPoolSize,
            MaxPoolSize,

            MultipleActiveResultSets,
            Replication,

            ConnectTimeout,
            Encrypt,
            TrustServerCertificate,
            LoadBalanceTimeout,
            PacketSize,
            TypeSystemVersion,

            ApplicationName,
            CurrentLanguage,
            WorkstationID,

            UserInstance,

            TransactionBinding,

            ApplicationIntent,

            MultiSubnetFailover,

            ConnectRetryCount,

            ConnectRetryInterval,

            // keep the count value last
            KeywordsCount
        }

        private sealed class SqlInitialCatalogConverter : StringConverter
        {
            // converter classes should have public ctor

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return GetStandardValuesSupportedInternal(context);
            }

            private bool GetStandardValuesSupportedInternal(ITypeDescriptorContext context)
            {
                // Only say standard values are supported if the connection string has enough
                // information set to instantiate a connection and retrieve a list of databases
                var flag = false;
                if (null == context) return flag;
                if (!(context.Instance is SqlConnectionStringBuilder constr)) return flag;
                if (0 < constr.DataSource.Length && (constr.IntegratedSecurity || 0 < constr.UserID.Length))
                    flag = true;

                return flag;
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                // Although theoretically this could be true, some people may want to just type in a name
                return false;
            }
        }
    }
}