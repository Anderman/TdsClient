using System.Linq;
using Medella.TdsClient.TdsStream.Native;
using Medella.TdsClient.TdsStream.SniNp;
using Medella.TdsClient.TdsStream.TcpIp;

namespace Medella.TdsClient.TdsStream
{
    public class TdsStreamProxy
    {
        private const string DefaultHostName = "localhost";
        private const string LocalDbHost = "(localdb)";
        private readonly bool _isLocalDb;
        private readonly bool _isLocalHost;
        private readonly bool _isTcpIp;
        private int _ipPort = -1;

        public TdsStreamProxy(string dataSource)
        {
            //datasource is localDb or tcp:servername,port
            var lowercaseDataSource = dataSource.Trim().ToLowerInvariant();
            _isLocalHost = IsLocalHost(lowercaseDataSource);
            _isLocalDb = IsLocalDbServer(lowercaseDataSource);
            _isTcpIp = IsTcpIp(lowercaseDataSource);
            if (_isLocalDb)
                SetNpProperties(lowercaseDataSource);
            else if (_isTcpIp)
                SetTcpProperties(lowercaseDataSource);
        }

        //TCp properties
        private string IpServerName { get; set; }
        private bool IsSsrpRequired { get; set; }

        //NamedPpipe  properties
        private string PipeName { get; set; }
        private string PipeServerName { get; set; }

        public ITdsStream CreateTdsStream(int timeout)
        {
            return _isLocalHost
                ? new TdsStreamNative(".", timeout)
                : _isLocalDb
                    ? new TdsStreamNamedpipes(PipeServerName, PipeName, timeout)
                    : _isTcpIp
                        ? new TdsStreamTcp(IpServerName, _ipPort, timeout)
                        : (ITdsStream) null;
        }

        public static bool IsLocalDbServer(string fullServername)
        {
            // All LocalDb endpoints are of the format host\instancename where host is always (LocalDb) (case-insensitive)
            var parts = fullServername.ToLowerInvariant().Split('\\');

            return parts.Length == 2 && LocalDbHost.Equals(parts[0].TrimStart());
        }

        private void SetTcpProperties(string lower)
        {
            var temp = lower.Split(':');
            temp = temp.Length == 2
                ? temp[1].Split(',')
                : lower.Split(',');
            if (temp.Length == 2) int.TryParse(temp[1], out _ipPort);

            IpServerName = temp[0].Split('\\')[0];
            temp = temp[0].Split('\\');
            if (temp.Length == 2 && _ipPort == -1)
                IsSsrpRequired = true;
            if (IsLocalHost(IpServerName)) IpServerName = DefaultHostName;
        }

        private void SetNpProperties(string fullServerName)
        {
            var protocolParts = GetNamedPipename(fullServerName).ToLower().Split('\\');

            PipeName = string.Join(@"\", protocolParts.Skip(4)); //localdb#678e2031\tsql\query
            PipeServerName = string.Join(@"\", protocolParts[2]); //.
        }

        public static string GetNamedPipename(string fullServername)
        {
            // All LocalDb endpoints are of the format host\instancename where host is always (LocalDb) (case-insensitive)
            var localDbInstance = fullServername.ToLowerInvariant().Split('\\')[1];
            return LocalDb.LocalDb.GetLocalDbConnectionString(localDbInstance);
        }

        public static bool IsTcpIp(string servername)
        {
            var temp = servername.Split(';');
            if (temp.Length > 2)
                return false;
            if (temp.Length == 2 && !int.TryParse(temp[1], out _))
                return false;
            temp = servername.Split(':');
            if (temp.Length > 2)
                return false;
            if (temp.Length == 2 && temp[0] != "tcp")
                return false;
            return true;
        }

        private static bool IsLocalHost(string serverName)
        {
            return string.IsNullOrEmpty(serverName) || ".".Equals(serverName) || "(local)".Equals(serverName) || "localhost".Equals(serverName);
        }
    }
}