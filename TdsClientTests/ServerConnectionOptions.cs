using System;
using System.Linq;
using System.Net;
using Medella.TdsClient.LocalDb;

namespace TdsClientTests
{
    public class ServerConnectionOptions
    {
        public string InstanceName;
        //TCp properties
        public string IpServerName;
        public int IpPort = -1;
        public bool IsSsrpRequired;
        //NamedPpipe  properties
        public string PipeName;
        public string PipeServerName;
        //cached intantcename for connect
        private const string SqlServerSpnHeader = "MSSQLSvc";
        private const int DefaultSqlServerPort = 1433;
        private const string DefaultHostName = "localhost";

        private const string LocalDbHost = "(localdb)";
        internal Protocol ConnectionProtocol = Protocol.None;

        public ServerConnectionOptions(string dataSource, bool isIntegratedSecurity)
        {
            //datasource is localDb or tcp:servername,port
            var lower = dataSource.Trim().ToLowerInvariant();
            if (IsLocalDbServer(lower))
                SetNpProperties(lower, isIntegratedSecurity);
            else if (IsTcpIp(lower))
                SetTcpProperties(lower, isIntegratedSecurity);
        }
        public static bool IsLocalDbServer(string fullServername)
        {
            // All LocalDb endpoints are of the format host\instancename where host is always (LocalDb) (case-insensitive)
            var parts = fullServername.ToLowerInvariant().Split('\\');

            return parts.Length == 2 && LocalDbHost.Equals(parts[0].TrimStart());
        }
        public static string GetNamedPipename(string fullServername)
        {
            // All LocalDb endpoints are of the format host\instancename where host is always (LocalDb) (case-insensitive)
            if (!IsLocalDbServer(fullServername)) throw new Exception("Not a localdb server");
            var localDbInstance = fullServername.ToLowerInvariant().Split('\\')[1];
            return LocalDb.GetLocalDbConnectionString(localDbInstance);
        }


        private void SetTcpProperties(string lower, bool isIntegratedSecurity)
        {
            var temp = lower.Split(':');
            temp = temp.Length == 2
                ? temp[1].Split(',')
                : lower.Split(',');
            if (temp.Length == 2)
            {
                int.TryParse(temp[1], out IpPort);
            }

            IpServerName = temp[0].Split('\\')[0];
            temp = temp[0].Split('\\');
            if (temp.Length == 2)
                InstanceName = temp[1];
            if (temp.Length == 2 && IpPort == -1)
                IsSsrpRequired = true;
            ConnectionProtocol = Protocol.TCP;
            if (isIntegratedSecurity)
                SetSqlServerSpn(IpServerName);
        }

        private void SetNpProperties(string fullServerName, bool isIntegratedSecurity)
        {
            var protocolParts = GetNamedPipename(fullServerName).ToLower().Split('\\');

            PipeName = string.Join(@"\", protocolParts.Skip(4)); //localdb#678e2031\tsql\query
            PipeServerName = string.Join(@"\", protocolParts[2]); //.
            InstanceName = "";
            ConnectionProtocol = Protocol.NP;
            if (isIntegratedSecurity)
                SetSqlServerSpn(PipeServerName);
        }


        public static bool IsNamedPipe(string[] protocolParts)
        {
            var parts = protocolParts;
            return parts.Length == 7
                   && parts[0] == "np:"
                   && parts[1] == ""
                   && parts[2] == "."
                   && parts[3] == "pipe"
                   && parts[4].Length > 0
                   && parts[5] == "tsql"
                   && parts[6] == "query";
        }

        public static bool IsTcpIp(string servername)
        {
            var temp = servername.Split(';');
            if (temp.Length > 2)
                return false;
            if (temp.Length == 2 && !int.TryParse(temp[1], out int _))
                return false;
            temp = servername.Split(':');
            if (temp.Length > 2)
                return false;
            if (temp.Length == 2 && temp[0] != "tcp")
                return false;
            return true;
        }
        private void SetSqlServerSpn(string serverName)
        {
            // If Server name is empty or a localhost name, then use "localhost"
            if (IsLocalHost(serverName))
                serverName = ConnectionProtocol == Protocol.Admin ? Environment.MachineName : DefaultHostName;
            var hostName = serverName;
            var portOrInstanceName = (IpPort != -1)
                ? IpPort.ToString()
                : (!string.IsNullOrWhiteSpace(InstanceName))
                    ? InstanceName
                    // For handling tcp:<hostname> format
                    : DefaultSqlServerPort.ToString();


            var sqlServerSpn = GetSqlServerSpn(hostName, portOrInstanceName);
        }
        private static bool IsLocalHost(string serverName)
        {
            return string.IsNullOrEmpty(serverName) || ".".Equals(serverName) || "(local)".Equals(serverName) || "localhost".Equals(serverName);
        }

        private static string GetSqlServerSpn(string hostNameOrAddress, string portOrInstanceName)
        {
            var hostEntry = Dns.GetHostEntry(hostNameOrAddress);
            var fullyQualifiedDomainName = hostEntry.HostName;
            var serverSpn = SqlServerSpnHeader + "/" + fullyQualifiedDomainName;
            if (!string.IsNullOrWhiteSpace(portOrInstanceName)) serverSpn += ":" + portOrInstanceName;
            return serverSpn;
        }
        internal enum Protocol
        {
            TCP,
            NP,
            None,
            Admin
        }
    }
}