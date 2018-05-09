using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Medella.TdsClient.SNI;
using Medella.TdsClient.SNI.Native;
using Medella.TdsClient.SNI.SniNp;
using Medella.TdsClient.SNI.TcpIp;

namespace Medella.TdsClient.TDS
{
    public class ServerConnectionOptions
    {
        //cached spn for login
        private const string SqlServerSpnHeader = "MSSQLSvc";
        private const string DefaultHostName = "localhost";
        private const string LocalDbHost = "(localdb)";
        private Protocol _connectionProtocol = Protocol.None;

        public string InstanceName { get; set; }

        //cached intantcename for connect


        public int IpPort = -1;
        private readonly bool _isLocalDb;
        private readonly bool _isTcpIp;

        //TCp properties
        public string IpServerName { get; set; }

        public bool IsSsrpRequired { get; set; }
        public string SqlServerSpn { get; set; }

        //NamedPpipe  properties
        private string PipeName { get; set; }
        private string PipeServerName { get; set; }
        //public SspiHelper Sspi { get; private set; }

        public ServerConnectionOptions(string dataSource)
        {
            //datasource is localDb or tcp:servername,port
            var lowercaseDataSource = dataSource.Trim().ToLowerInvariant();
            _isLocalDb = IsLocalDbServer(lowercaseDataSource);
            _isTcpIp = IsTcpIp(lowercaseDataSource);
            if (_isLocalDb)
                SetNpProperties(lowercaseDataSource);
            else if (_isTcpIp)
                SetTcpProperties(lowercaseDataSource);
        }

        public ISniHandle CreateTdsStream(int timeout)
        {
            //return new SniNative(IpServerName, 15);
            return _isLocalDb
                ? new SniNpHandle(PipeServerName, PipeName, timeout)
                : _isTcpIp
                    ? new SniTcpHandle(IpServerName,IpPort,timeout)
                    : (ISniHandle)null;
        }

        public byte[] InstanceNameBytes { get; set; }

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
            return LocalDb.LocalDb.GetLocalDbConnectionString(localDbInstance);
        }


        private void SetTcpProperties(string lower)
        {
            var temp = lower.Split(':');
            temp = temp.Length == 2
                ? temp[1].Split(',')
                : lower.Split(',');
            if (temp.Length == 2) int.TryParse(temp[1], out IpPort);

            IpServerName = temp[0].Split('\\')[0];
            temp = temp[0].Split('\\');
            if (temp.Length == 2)
                InstanceName = temp[1];
            if (temp.Length == 2 && IpPort == -1)
                IsSsrpRequired = true;
            _connectionProtocol = Protocol.TCP;
            if (IsLocalHost(IpServerName)) IpServerName = DefaultHostName;
        }

        private void SetNpProperties(string fullServerName)
        {
            var protocolParts = GetNamedPipename(fullServerName).ToLower().Split('\\');

            PipeName = string.Join(@"\", protocolParts.Skip(4)); //localdb#678e2031\tsql\query
            PipeServerName = string.Join(@"\", protocolParts[2]); //.
            InstanceName = "";
            _connectionProtocol = Protocol.NP;
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