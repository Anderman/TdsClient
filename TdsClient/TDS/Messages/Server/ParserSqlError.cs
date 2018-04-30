using System;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Package;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserSqlError
    {
        public static SqlInfoAndError SqlErrorAndInfo(this TdsPackageReader tdsPackageReader, byte token, int tokenLength)
        {
            var start = tdsPackageReader.GetReadPos();
           var error =new SqlInfoAndError
            {
                Number = tdsPackageReader.ReadInt32(),
                State = tdsPackageReader.ReadByte(),
                Class = tdsPackageReader.ReadByte(),
                Message = tdsPackageReader.ReadString(tdsPackageReader.ReadUInt16()),
                Server = tdsPackageReader.ReadString(tdsPackageReader.ReadByte()),
                Procedure = tdsPackageReader.ReadString(tdsPackageReader.ReadByte()),
            };
            var current = tdsPackageReader.GetReadPos();
            error.LineNumber = tokenLength - (current - start) > 2 ? tdsPackageReader.ReadInt32() : tdsPackageReader.ReadInt16();
            if (error.Class >= TdsEnums.MIN_ERROR_CLASS) throw new Exception(error.Message);
            return error;
        }
    }
}