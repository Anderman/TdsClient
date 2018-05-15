using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Package.Reader;
using Medella.TdsClient.TDS.Reader.StringHelpers;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserSqlColMetadataBulkCopy
    {
        public static MetadataBulkCopy[] ColMetaDataBulkCopy(this TdsPackageReader reader, int columns)
        {
            reader.InitNbcBitmap(columns);

            var newMetaData = new MetadataBulkCopy[columns];
            for (var i = 0; i < columns; i++)
                newMetaData[i] = ReadMetadata(reader);
            return newMetaData;
        }

        private static MetadataBulkCopy ReadMetadata(TdsPackageReader reader)
        {
            var col = new MetadataBulkCopy();
            // read user type - 4 bytes Yukon, 2 backwards
            reader.ReadUInt32();

            // read the 2 flags and set appropriate flags in structure
            col.Flag1 = reader.ReadByte();
            col.Flag2 = reader.ReadByte();

            var tdsType = reader.ReadByte();
            col.TdsType = tdsType;

            var tmp = col.MetaType = TdsMetaType.TdsTypes[tdsType];

            col.IsPlp = tmp.IsPlp;
            col.IsTextOrImage = tmp.IsTextOrImage;

            var length = col.Length = reader.ReadTdsTypeLen(tmp.LenBytes);
            if (length == TdsEnums.SQL_USHORTVARMAXLEN && tmp.LenBytes == 2)
                col.IsPlp = true;

            if (tdsType == TdsEnums.SQLUDT)
                reader.ReadUdtMetadata();

            if (tdsType == TdsEnums.SQLXMLTYPE)
                reader.ReadXmlSchema();

            if (tmp.HasPrecision)
                col.Precision = reader.ReadByte();

            if (tmp.HasScale)
                col.Scale = reader.ReadByte();

            if (tmp.HasCollation)
            {
                col.Collation = reader.ReadCollation();
                col.Encoding = reader.CurrentSession.GetEncodingFromCache(col.Collation.GetCodePage());
            }

            if (col.IsTextOrImage)
                col.PartTableName = reader.ReadMultiPartTableName();

            //bulkcopy typeCorrection
            if (tdsType == TdsEnums.SQLXMLTYPE)
            {
                col.TdsType = TdsEnums.SQLNVARCHAR;
                col.Length = TdsEnums.SQL_USHORTVARMAXLEN;
                col.Collation = new SqlCollations();
            }
            if (tdsType == TdsEnums.SQLUDT)
            {
                col.TdsType = TdsEnums.SQLBIGVARBINARY;
                col.Length = TdsEnums.SQL_USHORTVARMAXLEN;
            }

            col.Column = reader.ReadString(reader.ReadByte());
            return col;
        }
    }
}