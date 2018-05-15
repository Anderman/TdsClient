using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Package.Reader;

namespace SqlClient.TDS.Messages.Server.Environment
{
    public static class ParserEnvLanguage
    {
        internal static void EnvLanguage(this TdsPackageReader tdsPackageReader)
        {
            var newValue = tdsPackageReader.ReadString(tdsPackageReader.ReadByte());
            var oldValue = tdsPackageReader.ReadString(tdsPackageReader.ReadByte());
        }
    }
}