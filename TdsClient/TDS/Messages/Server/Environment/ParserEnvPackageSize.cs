﻿using System.Globalization;
using Medella.TdsClient.SNI;
using Medella.TdsClient.TDS.Package;

namespace SqlClient.TDS.Messages.Server.Environment
{
    public static class ParserEnvPackageSize
    {
        public static void EnvPackageSize(this TdsPackageReader tdsPackageReader)
        {
            var newValue = tdsPackageReader.ReadString(tdsPackageReader.ReadByte());
            var oldValue = tdsPackageReader.ReadString(tdsPackageReader.ReadByte());
            var packetSize = int.Parse(newValue, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }
    }
}