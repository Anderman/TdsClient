using System.Globalization;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Package;

namespace Medella.TdsClient.TDS.Messages.Server.Environment
{
    public static class ParserEnvCharset
    {
        internal static void EnvCharset(this TdsPackageReader reader)
        {
            var newValue = reader.ReadString(reader.ReadByte());
            var oldValue = reader.ReadString(reader.ReadByte());
            // we copied this behavior directly from luxor - see charset envchange
            // section from sqlctokn.c
            if (newValue == TdsEnums.DEFAULT_ENGLISH_CODE_PAGE_STRING)
            {
                reader.CurrentSession.DefaultCodePage = TdsEnums.DEFAULT_ENGLISH_CODE_PAGE_VALUE;
                reader.CurrentSession.DefaultEncoding = Encoding.GetEncoding(reader.CurrentSession.DefaultCodePage);
                return;
            }
            var stringCodePage = newValue.Substring(TdsEnums.CHARSET_CODE_PAGE_OFFSET);
            reader.CurrentSession.DefaultCodePage = int.Parse(stringCodePage, NumberStyles.Integer, CultureInfo.InvariantCulture);
            reader.CurrentSession.DefaultEncoding = Encoding.GetEncoding(reader.CurrentSession.DefaultCodePage);
        }
    }
}