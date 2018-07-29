using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Controller;
using Medella.TdsClient.TDS.Messages.Client;
using Medella.TdsClient.TDS.Messages.Server;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Package.Writer;
using Medella.TdsClient.TDS.Row.Writer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medella.TdsClient.TDS.Processes
{
    public static class BulkInsertExtentions
    {
        private static readonly byte[] Done = { 0xFD, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        public static void BulkInsert<T>(this TdsConnection cnn, IEnumerable<T> objects, string tableName)
        {
            cnn.BulkInsert(objects, tableName, null);
        }
        public static void BulkInsert<T>(this TdsConnection cnn, IEnumerable<T> objects, string tableName, Func<MetadataBulkCopy[], MetadataBulkCopy[]> columnmapping)
        {
            var writer = cnn.TdsPackage.Writer;
            var reader = cnn.TdsPackage.Reader;
            var parser = cnn.StreamParser;
            MetadataBulkCopy[] metaDataAllColumns = null;

            writer.SendExcuteBatch($"SET FMTONLY ON select * from {tableName} SET FMTONLY OFF", cnn.SqlTransactionId);
            parser.ParseInput(count => { metaDataAllColumns = reader.ColMetaDataBulkCopy(count); });

            writer.ColumnsMetadata = columnmapping != null ? columnmapping(GetUsedColumns(metaDataAllColumns)) : GetUsedColumns(metaDataAllColumns);

            var bulkInsert = CreateBulkInsertStatement(tableName, writer.ColumnsMetadata);
            writer.SendExcuteBatch(bulkInsert, cnn.SqlTransactionId);
            parser.ParseInput();

            writer.NewPackage(TdsEnums.MT_BULK);
            var columnWriter = new TdsColumnWriter(writer);
            var rowWriter = RowWriter.GetComplexWriter<T>(columnWriter);
            WriteBulkInsertColMetaData(writer);

            foreach (var o in objects)
            {
                writer.WriteByte(TdsEnums.SQLROW);
                rowWriter(columnWriter, o);
            }

            writer.WriteByteArray(Done);
            writer.SendLastMessage();
            parser.ParseInput();
            if (parser.Status != ParseStatus.Done)
                parser.ParseInput();
        }


        private static MetadataBulkCopy[] GetUsedColumns(MetadataBulkCopy[] metadataAllColumns)
        {
            return metadataAllColumns.Where(metadata => metadata.TdsType != TdsEnums.SQLTIMESTAMP && !metadata.IsIdentity).ToArray();
        }

        private static string CreateBulkInsertStatement(string tableName, MetadataBulkCopy[] columns)
        {
            var seperator = "";

            var sb = new StringBuilder("insert bulk ");
            sb.Append("[");
            sb.Append(tableName);
            sb.Append("]");
            sb.Append(" (");

            foreach (var metadata in columns)
            {
                var value = CreateParameter(metadata);
                sb.Append(seperator);
                sb.Append("[");
                sb.Append(EscapeIdentifier(metadata.Column));
                sb.Append("]");
                sb.Append(value);
                seperator = ",";
            }

            sb.Append(")");

            return sb.ToString();
        }

        public static void WriteBulkInsertColMetaData(TdsPackageWriter writer)
        {
            //0x81,0x01,0x00,//colmetadata 0x001 column
            //0x00,0x00,0x00,0x00,//usertype ignore
            //0x09,0x00, //flags
            //0x26,0x04, //int 4
            //0x02,0x49,0x00,0x64,0x00, //columnname 2 chars ID
            var columns = writer.ColumnsMetadata;
            writer.WriteByte(TdsEnums.SQLCOLMETADATA);
            writer.WriteInt16(columns.Length);
            foreach (var column in columns)
                WriteColumn(writer, column);
        }

        private static void WriteColumn(TdsPackageWriter writer, MetadataBulkCopy md)
        {
            writer.WriteInt32(0x0);

            writer.WriteByte(md.Flag1);
            writer.WriteByte(md.Flag2);
            writer.WriteByte(md.TdsType);
            var mt = TdsMetaType.TdsTypes[md.TdsType];
            if (mt.LenBytes == 1) writer.WriteByte(md.Length);
            if (mt.LenBytes == 2) writer.WriteInt16(md.Length);
            if (mt.LenBytes == 4) writer.WriteInt32(md.Length);

            if (mt.HasPrecision)
                writer.WriteByte(md.Precision);
            if (mt.HasScale)
                writer.WriteByte(md.Scale);
            if (mt.HasCollation)
            {
                writer.WriteUInt32(md.Collation.Info);
                writer.WriteByte(md.Collation.SortId);
            }

            if (mt.IsTextOrImage)
            {
                writer.WriteInt16(md.PartTableName.TableName.Length);
                writer.WriteUnicodeString(md.PartTableName.TableName);
            }

            writer.WriteByte((byte)md.Column.Length);
            writer.WriteUnicodeString(md.Column);
        }

        private static string CreateParameter(MetadataBulkCopy mt)
        {
            var size = mt.IsPlp ? "max" : mt.Length.ToString();
            switch (mt.TdsType)
            {
                case TdsEnums.SQLBIT: return " Bit";
                case TdsEnums.SQLBITN: return " Bit";
                case TdsEnums.SQLINT1: return " TinyInt";
                case TdsEnums.SQLINT2: return " SmallInt";
                case TdsEnums.SQLINT4: return " Int";
                case TdsEnums.SQLINT8: return " BigInt";
                case TdsEnums.SQLINTN:
                    if (mt.Length == 1) goto case TdsEnums.SQLINT1;
                    if (mt.Length == 2) goto case TdsEnums.SQLINT2;
                    if (mt.Length == 4) goto case TdsEnums.SQLINT4;
                    if (mt.Length == 8) goto case TdsEnums.SQLINT8;
                    throw new Exception("Unknown length of SQLINTN");
                case TdsEnums.SQLMONEY4: return " SmallMoney";
                case TdsEnums.SQLMONEY: return " Money";
                case TdsEnums.SQLMONEYN:
                    if (mt.Length == 4) goto case TdsEnums.SQLMONEY4;
                    if (mt.Length == 8) goto case TdsEnums.SQLMONEY;
                    throw new Exception("Unknown length of SQLMONEYN");
                case TdsEnums.SQLFLT4: return " Real";
                case TdsEnums.SQLFLT8: return " Float";
                case TdsEnums.SQLFLTN:
                    if (mt.Length == 4) goto case TdsEnums.SQLFLT4;
                    if (mt.Length == 8) goto case TdsEnums.SQLFLT8;
                    throw new Exception("Unknown length of SQLFLTN");
                case TdsEnums.SQLDATETIME2: return $" DateTime2({mt.Scale})";
                case TdsEnums.SQLDATETIMEOFFSET: return $" DateTimeOffset({mt.Scale})";
                case TdsEnums.SQLDATETIM4: return " SmallDateTime";
                case TdsEnums.SQLDATETIME: return " DateTime";
                case TdsEnums.SQLDATETIMN:
                    if (mt.Length == 4) goto case TdsEnums.SQLDATETIM4;
                    if (mt.Length == 8) goto case TdsEnums.SQLDATETIME;
                    throw new Exception("Unknown length of SQLDATETIMN");
                case TdsEnums.SQLDATE: return " Date";
                case TdsEnums.SQLTIME: return $" Time({mt.Scale})";
                case TdsEnums.SQLDECIMALN: return $" Decimal({mt.Precision},{mt.Scale})";
                case TdsEnums.SQLUNIQUEID: return " UniqueIdentifier";
                case TdsEnums.SQLBIGBINARY: return $" Binary({mt.Length})";
                case TdsEnums.SQLBIGVARBINARY: return $" VarBinary({size})";
                case TdsEnums.SQLBIGCHAR: return $" Char({mt.Length})";
                case TdsEnums.SQLBIGVARCHAR: return $" VarChar({size})";
                case TdsEnums.SQLNCHAR: return $" NChar({mt.Length})";
                case TdsEnums.SQLNVARCHAR: return $" NVarChar({size})";
                case TdsEnums.SQLVARIANT: return " Sql_variant";
                case TdsEnums.SQLIMAGE: return " Image";
                case TdsEnums.SQLTEXT: return " Text";
                case TdsEnums.SQLNTEXT: return " NText";
            }

            throw new Exception("TdsType Unknown");
        }

        internal static string EscapeIdentifier(string name)
        {
            return name.Replace("]", "]]");
        }
    }
}