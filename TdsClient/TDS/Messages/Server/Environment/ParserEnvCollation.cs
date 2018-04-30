using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Medella.TdsClient.SNI;
using Medella.TdsClient.TDS.Messages.Server;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Reader.StringHelpers;

namespace SqlClient.TDS.Messages.Server.Environment
{
    public static class ParserEnvCollation
    {
        internal static void EnvCollation(this TdsPackageReader reader)
        {
            var newLength = reader.ReadByte();
            if (newLength == 5)
            {
                reader.CurrentSession.DefaultCollation = reader.ReadCollation();
                var newCodePage = reader.CurrentSession.DefaultCollation.GetCodePage();
                if (newCodePage != reader.CurrentSession.DefaultCodePage)
                {
                    reader.CurrentSession.DefaultCodePage = newCodePage;
                    reader.CurrentSession.DefaultEncoding = Encoding.GetEncoding(newCodePage);
                }
            }

            var oldLength = reader.ReadByte();
            if (oldLength == 5)
            {
                var oldCollation = reader.ReadCollation();
            }
        }
    }
}