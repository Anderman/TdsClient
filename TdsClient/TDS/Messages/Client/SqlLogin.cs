using Medella.TdsClient.Cleanup;
using Medella.TdsClient.Contants;

namespace Medella.TdsClient.TDS.Messages.Client
{
    public sealed class SqlLogin
    {
        public string applicationName = ""; // application name
        public string attachDBFilename = ""; // DB filename to be attached
        public string database = ""; // initial database
        public string hostName = ""; // client machine name
        public string language = ""; // initial language
        public int packetSize = SqlConnectionString.DEFAULT.Packet_Size; // packet size
        public string password = ""; // password
        public bool ReadOnlyIntent = false; // read-only intent
        public string serverName = ""; // server name
        public int timeout; // login timeout
        public bool useReplication = false; // user login for replication
        public bool userInstance = false; // user instance
        public string userName = ""; // user id
        public bool useSSPI = false; // use integrated security
        public TdsEnums.FeatureExtension RequestedFeatures { get; set; }

        public byte[] ClientToken { get; set; }

        //public SspiHelper Sspi { get; set; }
    }
}