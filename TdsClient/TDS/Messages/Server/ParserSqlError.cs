using System;
using Medella.TdsClient.Contants;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Package.Reader;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserSqlError
    {
        public static void SqlErrorAndInfo(this TdsPackageReader reader, byte token, int tokenLength)
        {
            var start = reader.GetReadPos();
           var error =new SqlInfoAndError
            {
                Number = reader.ReadInt32(),
                State = reader.ReadByte(),
                Class = reader.ReadByte(),
                Message = reader.ReadString(reader.ReadUInt16()),
                Server = reader.ReadString(reader.ReadByte()),
                Procedure = reader.ReadString(reader.ReadByte()),
            };
            var current = reader.GetReadPos();
            error.LineNumber = tokenLength - (current - start) > 2 ? reader.ReadInt32() : reader.ReadInt16();
            if (error.Class >= TdsEnums.MIN_ERROR_CLASS)
                throw new Exception(error.Message);
            reader.CurrentSession.Errors.Add(error);
        }
    }
}