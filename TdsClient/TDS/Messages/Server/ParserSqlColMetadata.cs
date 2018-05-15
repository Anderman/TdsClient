using System;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Package.Reader;
using Medella.TdsClient.TDS.Row.Reader.StringHelpers;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserSqlColMetadata
    {
        public static void ColMetaData(this TdsPackageReader reader, int columns)
        {
            reader.InitNbcBitmap(columns);

            var newMetaData = new ColumnsMetadata(columns);
            for (var i = 0; i < columns; i++)
                ReadMetadata(reader, newMetaData[i]);
            reader.CurrentResultset.ColumnsMetadata = newMetaData;
        }

        private static void ReadMetadata(TdsPackageReader reader, ColumnMetadata col)
        {
            // read user type - 4 bytes Yukon, 2 backwards
            reader.ReadUInt32();

            // read the 2 flags and set appropriate flags in structure
            col.Flag1 = reader.ReadByte();
            col.Flag2 = reader.ReadByte();

            var tdsType = col.TdsType = reader.ReadByte();

            var tmp = col.MetaType= TdsMetaType.TdsTypes[tdsType];
            col.IsPlp = tmp.IsPlp;
            col.IsTextOrImage = tmp.IsTextOrImage;


            var length = ReadTdsTypeLen(reader, tmp.LenBytes);
            if (length == TdsEnums.SQL_USHORTVARMAXLEN && tmp.LenBytes == 2)
                col.IsPlp = true;

            if (tdsType == TdsEnums.SQLUDT)
                reader.ReadUdtMetadata();

            if (tdsType == TdsEnums.SQLXMLTYPE)
                ReadXmlSchema(reader);

            if (tmp.HasPrecision)
                reader.ReadByte();

            if (tmp.HasScale)
                col.Scale = reader.ReadByte();

            if (tmp.HasCollation)
                col.Encoding = GetEncodingFromCollation(reader);

            if (col.TdsType == TdsEnums.SQLTEXT || col.TdsType == TdsEnums.SQLNTEXT || col.TdsType == TdsEnums.SQLIMAGE)
                ReadMultiPartTableName(reader);

            col.Column = reader.ReadString(reader.ReadByte());
        }

        public static XmlSchema ReadXmlSchema(this TdsPackageReader reader)
        {
            var schemapresent = reader.ReadByte();
            if ((schemapresent & 1) != 0)
                return new XmlSchema
                {
                    CollectionDatabase = reader.ReadString(reader.ReadByte()),
                    CollectionOwningSchema = reader.ReadString(reader.ReadByte()),
                    CollectionName = reader.ReadString(reader.ReadUInt16())
                };
            return null;
        }

        public static Encoding GetEncodingFromCollation(this TdsPackageReader reader)
        {
            var collation = reader.ReadCollation();
            var codePage = collation.GetCodePage();
            return reader.CurrentSession.GetEncodingFromCache(codePage);
        }

        public static int ReadTdsTypeLen(this TdsPackageReader reader, int len)
        {
            return len == 0
                ? 0
                : len == 1
                    ? reader.ReadByte()
                    : len == 2
                        ? reader.ReadUInt16()
                        : reader.ReadInt32();
        }

        public static MultiPartTableName ReadMultiPartTableName(this TdsPackageReader reader)
        {
            // Find out how many parts in the TDS stream
            var nParts = reader.ReadByte();
            if (nParts == 0)
                return null;

            var mpt = new MultiPartTableName();
            if (nParts == 4) mpt.ServerName = reader.ReadString(reader.ReadUInt16());

            if (nParts >= 3) mpt.CatalogName = reader.ReadString(reader.ReadUInt16());

            if (nParts >= 2) mpt.SchemaName = reader.ReadString(reader.ReadUInt16());

            mpt.TableName = reader.ReadString(reader.ReadUInt16());

            return mpt;
        }

        internal static Udt ReadUdtMetadata(this TdsPackageReader reader)
        {
            return new Udt
            {
                DatabaseName = reader.ReadString(reader.ReadByte()),
                SchemaName = reader.ReadString(reader.ReadByte()),
                TypeName = reader.ReadString(reader.ReadByte()),
                AssemblyQualifiedName = reader.ReadString(reader.ReadUInt16())
            };
        }
    }

    public class Udt
    {
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string TypeName { get; set; }
        public string AssemblyQualifiedName { get; set; }
    }
}