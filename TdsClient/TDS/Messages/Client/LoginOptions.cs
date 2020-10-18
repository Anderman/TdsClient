using Medella.TdsClient.Cleanup;
using Medella.TdsClient.Constants;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public sealed class LoginOptions
    {
        public string ApplicationName = ""; // application name
        public string AttachDbFilename = ""; // DB filename to be attached
        public string Database = ""; // initial database
        public string HostName = ""; // client machine name
        public string Language = ""; // initial language
        public int PacketSize = SqlConnectionString.DEFAULT.Packet_Size; // packet size
        public string Password = ""; // password
        public bool ReadOnlyIntent = false; // read-only intent
        public string ServerName = ""; // server name
        public int timeout; // login timeout
        public bool UseReplication = false; // user login for replication
        public bool UserInstance = false; // user instance
        public string UserName = ""; // user id
        public bool UseSspi = false; // use integrated security
        public TdsEnums.FeatureExtension RequestedFeatures { get; set; }

        public byte[] ClientToken { get; set; }
    }
}