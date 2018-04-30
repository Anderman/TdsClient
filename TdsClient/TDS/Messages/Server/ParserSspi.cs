using Medella.TdsClient.TDS.Controller.Sspi;
using Medella.TdsClient.TDS.Package;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserSspi
    {
        public static void ParseToken(this TdsPackageReader reader, SspiHelper sspi, int length)
        {
            var token = reader.GetBytes(length);
            var buf = token.ToArray();
            sspi.CreateClientToken(buf);
        }
    }
}