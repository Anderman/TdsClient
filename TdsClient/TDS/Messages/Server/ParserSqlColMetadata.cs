using System;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Reader.StringHelpers;
using SqlClient.TDS.Messages.Server;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserSqlColMetadata
    {
        private static readonly ConcurrentDictionary<int, Encoding> EncodingCache = new ConcurrentDictionary<int, Encoding>();

        public static ColumnsMetadata ColMetaData(this TdsPackageReader reader, int columns)
        {
            reader.InitNbcBitmap(columns);

            var newMetaData = new ColumnsMetadata(columns);
            for (var i = 0; i < columns; i++)
                ReadMetadata(reader, newMetaData[i]);
            return newMetaData;
        }

        private static void ReadMetadata(TdsPackageReader reader, ColumnMetadata col)
        {
            // read user type - 4 bytes Yukon, 2 backwards
            reader.ReadUInt32();

            // read the 2 flags and set appropriate flags in structure
            var flags = reader.ReadByte();
            col.Updatability = (byte)((flags & TdsEnums.Updatability) >> 2);
            col.IsNullable = TdsEnums.Nullable == (flags & TdsEnums.Nullable);
            col.IsIdentity = TdsEnums.Identity == (flags & TdsEnums.Identity);
            flags = reader.ReadByte();
            col.IsColumnSet = TdsEnums.IsColumnSet == (flags & TdsEnums.IsColumnSet);

            var tdsType = reader.ReadByte();

            col.TdsType = tdsType;

            var tmp = ReadMetaType.TdsMetaTypeRead[tdsType];
            col.IsPlp = tmp.IsPlp;
            col.IsLong = tmp.IsLong;
            col.SqlDbType = tmp.SqlDbType;

            var shortLength = ReadTdsTypeLen(reader, tdsType);
            if (shortLength == TdsEnums.SQL_USHORTVARMAXLEN)
            {
                col.IsPlp = true;
                col.IsLong = true;
            }

            if (tdsType == TdsEnums.SQLXMLTYPE)
            {
                var schemapresent = reader.ReadByte();

                if ((schemapresent & 1) != 0)
                {
                    var strLen = reader.ReadByte();
                    if (strLen != 0)
                        col.XmlSchemaCollectionDatabase = reader.ReadString(strLen);

                    strLen = reader.ReadByte();
                    if (strLen != 0)
                        col.XmlSchemaCollectionOwningSchema = reader.ReadString(strLen);

                    var shortLen = reader.ReadUInt16();
                    if (shortLen != 0)
                        col.XmlSchemaCollectionName = reader.ReadString(shortLen);
                }
            }

            if (tmp.HasPrecision)
                reader.ReadByte();// precision
            if (tmp.HasScale)
                col.Scale = reader.ReadByte();

            // read the collation for 7.x servers.
            // We need a conversion from collation to a encoding when we reader string data
            if (tmp.HasCollation)
                col.Encoding = GetEncodingFromCollation(reader);

            if (col.IsLong && !col.IsPlp)
                col.PartTableName = TryProcessOneTable(reader);
            var byteLen = reader.ReadByte();
            col.Column = reader.ReadString(byteLen);


            // We get too many DONE COUNTs from the server, causing too many StatementCompleted event firings.
            // We only need to fire this event when we actually have a meta data stream with 0 or more rows.
        }

        private static Encoding GetEncodingFromCollation(TdsPackageReader reader)
        {
            var collation = reader.ReadCollation();
            var codePage = collation.GetCodePage();

            var colEncoding = EncodingCache.GetOrAdd(codePage, x => Encoding.GetEncoding(codePage));
            return colEncoding;
        }

        public static int ReadTdsTypeLen(TdsPackageReader tdsPackageReader, byte tdsType)
        {
            switch (tdsType)
            {
                case TdsEnums.SQLTIME:
                case TdsEnums.SQLDATETIME2:
                case TdsEnums.SQLDATETIMEOFFSET:
                case TdsEnums.SQLDATE:
                    return 0;
            }

            if ((tdsType & TdsEnums.SQLLenMask) != TdsEnums.SQLVarLen)
                return 0;
            if ((tdsType & 0x80) != 0)
                return tdsPackageReader.ReadUInt16();
            if ((tdsType & 0x0c) == 0)
            {
                tdsPackageReader.ReadInt32();
                return 0;
            }
            tdsPackageReader.ReadByte();
            return 0;
        }


        private static MultiPartTableName TryProcessOneTable(TdsPackageReader tdsPackageReader)
        {
            ushort tableLen;
            string value;
            var mpt = new MultiPartTableName();
            // Find out how many parts in the TDS stream
            var nParts = tdsPackageReader.ReadByte();

            if (nParts == 4)
            {
                tableLen = tdsPackageReader.ReadUInt16();
                value = tdsPackageReader.ReadString(tableLen);
                mpt.ServerName = value;
                nParts--;
            }

            if (nParts == 3)
            {
                tableLen = tdsPackageReader.ReadUInt16();
                value = tdsPackageReader.ReadString(tableLen);
                mpt.CatalogName = value;
                nParts--;
            }

            if (nParts == 2)
            {
                tableLen = tdsPackageReader.ReadUInt16();
                value = tdsPackageReader.ReadString(tableLen);
                mpt.SchemaName = value;
                nParts--;
            }

            if (nParts == 1)
            {
                tableLen = tdsPackageReader.ReadUInt16();
                value = tdsPackageReader.ReadString(tableLen);
                mpt.TableName = value;
            }

            return mpt;
        }
    }
}