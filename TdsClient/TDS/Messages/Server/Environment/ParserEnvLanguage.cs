using Medella.TdsClient.TDS.Package.Reader;

namespace Medella.TdsClient.TDS.Messages.Server.Environment
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