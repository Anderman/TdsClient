using System.Linq;
using Medella.TdsClient.TdsStream.Native;
using Medella.TdsClient.TdsStream.SniNp;
using Medella.TdsClient.TdsStream.TcpIp;

namespace Medella.TdsClient.TdsStream
{
    public static class TdsStreamProxy
    {
        private const string DefaultHostName = "localhost";
        private const string LocalDbHost = "(localdb)";

        public static ITdsStream? CreatedStream(string dataSource, int timeoutSeconds)
        {
            var lowercaseDataSource = dataSource.Trim().ToLowerInvariant();
            if (IsLocalHost(lowercaseDataSource))
                return new TdsStreamNative(".", timeoutSeconds);
            if (IsLocalDbServer(lowercaseDataSource))
            {
                var (pipeName, serverNameNp) = GetNpProperties(lowercaseDataSource);
                return new TdsStreamNamedPipes(serverNameNp, pipeName, timeoutSeconds);
            }

            if (!IsTcpIp(lowercaseDataSource))
                return null;

            var (port, serverNameIp, _) = GetTcpProperties(lowercaseDataSource);
            return new TdsStreamTcp(serverNameIp, port, timeoutSeconds);
        }

        public static bool IsLocalDbServer(string fullServerName)
        {
            // All LocalDb endpoints are of the format host\instanceName where host is always (LocalDb) (case-insensitive)
            var parts = fullServerName.ToLowerInvariant().Split('\\');

            return parts.Length == 2 && LocalDbHost.Equals(parts[0].TrimStart());
        }

        //serverName=[tcp:]hostname[/instance][,port]
        private static (int port, string serverName, bool isSsrpRequired) GetTcpProperties(string lower)
        {
            var port = -1;
            var temp = lower.Split(':');
            temp = temp.Length == 2
                ? temp[1].Split(',')
                : lower.Split(',');
            if (temp.Length == 2) int.TryParse(temp[1], out port);

            temp = temp[0].Split('\\');
            var serverName = temp[0];
            var isSsrpRequired = temp.Length == 2 && port == -1;
            if (IsLocalHost(serverName)) serverName = DefaultHostName;
            return (port, serverName, isSsrpRequired);
        }

        public static (string pipeName, string ServerName) GetNpProperties(string fullServerName)
        {
            var protocolParts = GetNamedPipeName(fullServerName).ToLower().Split('\\');

            var pipeName = string.Join(@"\", protocolParts.Skip(4)); //localdb#678e2031\tsql\query
            var serverName = string.Join(@"\", protocolParts[2]); //.
            return (pipeName, serverName);
        }

        public static string GetNamedPipeName(string fullServerName)
        {
            // All LocalDb endpoints are of the format host\instancename where host is always (LocalDb) (case-insensitive)
            var localDbInstance = fullServerName.ToLowerInvariant().Split('\\')[1];
            return LocalDb.LocalDb.GetLocalDbConnectionString(localDbInstance)!;
        }

        //tcp:hostname,port
        public static bool IsTcpIp(string serverName)
        {
            var temp = serverName.Split(';');
            if (temp.Length > 2)
                return false;
            if (temp.Length == 2 && !int.TryParse(temp[1], out _))
                return false;
            temp = serverName.Split(':');
            if (temp.Length > 2)
                return false;
            if (temp.Length == 2 && temp[0] != "tcp")
                return false;
            return true;
        }

        private static bool IsLocalHost(string serverName) => string.IsNullOrEmpty(serverName) || ".".Equals(serverName) || "(local)".Equals(serverName) || "localhost".Equals(serverName);
    }
}