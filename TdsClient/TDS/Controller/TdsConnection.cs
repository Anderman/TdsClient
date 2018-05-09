using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medella.TdsClient.Cleanup;
using Medella.TdsClient.Contants;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.SNI;
using Medella.TdsClient.SNI.SniNp;
using Medella.TdsClient.TDS.Messages.Client;
using Medella.TdsClient.TDS.Messages.Server;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Reader;

namespace Medella.TdsClient.TDS.Controller
{
    public enum ParseStatus
    {
        Row,
        Done,
        AltRow,
        Unknown,
        BeforeFirstColumn,
        AfterLastColumn
    }

    public class TdsConnection : IDisposable
    {
        private readonly ISniHandle _tdsStream;
        private readonly string _connectionString;
        private bool _errorReceived;
        public ParseStatus Status;
        private readonly TdsPackage _tdsPackage;
        public List<SqlInfoAndError> SqlMessages => _tdsPackage.Reader.CurrentSession.Errors;
        private static readonly ConcurrentDictionary<string, Delegate> Readers = new ConcurrentDictionary<string, Delegate>();
        private readonly int _messageCountAfterlogin;
        private readonly TdsStreamParser _streamParser;

        static TdsConnection()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public TdsConnection(ISniHandle tdsStream, SqlConnectionString dbConnectionOptions)
        {
            _tdsStream = tdsStream;
            _connectionString = dbConnectionOptions.ConnectionString;
            _tdsPackage = new TdsPackage(tdsStream);
            var loginProcessor = new LoginProcessor(_tdsPackage, dbConnectionOptions);
            _streamParser = new TdsStreamParser(_tdsPackage, loginProcessor);
            _streamParser.ParseInput();
            _messageCountAfterlogin = SqlMessages.Count;
        }

        public void ExecuteNonQuery(string text)
        {
            _tdsPackage.Writer.SendExcuteBatch(text);
            _streamParser.ParseInput();
        }

        public List<T> ExecuteParameterQuery<T>(FormattableString text) where T : class, new()
        {
            _tdsPackage.Writer.SendRpc(_tdsPackage.Reader.CurrentSession.DefaultCollation, text);
            _streamParser.ParseInput();
            if (_streamParser.Status == ParseStatus.Done)
                return null;
            var r = new List<T>();
            var rowReader = GetRowReader<T>(text.Format);
            var columnReader = new TdsColumnReader(_tdsPackage.Reader);
            while (_streamParser.Status != ParseStatus.Done)
            {
                var result = rowReader(columnReader);
                _streamParser.ParseInput();
                r.Add(result);
            }

            return r;
        }
        private readonly byte[] _done = { 0xFD, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public void BulkInsert()
        {
            var writer = _tdsPackage.Writer;
            string tableName = "Bulkcopy";
            MetadataBulkCopy[] metaDataAllColumns = null;
            ExecuteNonQuery(@"IF object_id('bulkcopy') is not null drop table bulkcopy; Create table bulkcopy (Id int, Id1 Bigint)");
            _tdsPackage.Writer.SendExcuteBatch($"SET FMTONLY ON select * from {tableName} SET FMTONLY OFF");
            _streamParser.ParseInput(count =>
            {
                metaDataAllColumns = _tdsPackage.Reader.ColMetaDataBulkCopy(count);
            });
            writer.ColumnsMetadata = GetUsedColumns(metaDataAllColumns).ToArray();
            var bulkInsert = CreateBulkInsertStatement(tableName, writer.ColumnsMetadata);
            ExecuteNonQuery(bulkInsert);
            WriteBulkInsertColMetaData(_tdsPackage.Writer);
            for (int i = 0; i < 10; i++)
            {
                _tdsPackage.Writer.WriteByte(TdsEnums.SQLROW);
                for (int j = 0; j < 1; j++)
                {
                    WriteBulkInt32(_tdsPackage.Writer, (int?)2);
                    WriteBulkInt32(_tdsPackage.Writer, (int?)2);
                }
            }
            _tdsPackage.Writer.WriteByteArray(_done);
            _tdsPackage.Writer.SetHeader(TdsEnums.ST_EOM, TdsEnums.MT_BULK);
            _tdsPackage.Writer.FlushBuffer();
            _streamParser.ParseInput();
        }


        private List<MetadataBulkCopy> GetUsedColumns(MetadataBulkCopy[] metadataAllColumns)
        {
            return metadataAllColumns.Where(metadata => metadata.TdsType != TdsEnums.SQLTIMESTAMP && !metadata.IsIdentity).ToList();
        }

        private string CreateBulkInsertStatement(string tableName, MetadataBulkCopy[] columns)
        {
            var seperator = "";

            var sb = new StringBuilder("insert bulk ");
            sb.Append(tableName);
            sb.Append(" (");

            foreach (var metadata in columns)
            {
                {
                    var value = CreateParameter(metadata);
                    sb.Append(seperator);
                    sb.Append(EscapeIdentifier(metadata.Column));
                    sb.Append(value);
                    seperator = ",";
                }
            }
            sb.Append(")");

            return sb.ToString();
        }

        public static void WriteBulkInsertColMetaData(TdsPackageWriter writer )
        {
            //0x81,0x01,0x00,//colmetadata 0x001 column
            //0x00,0x00,0x00,0x00,//usertype ignore
            //0x09,0x00, //flags
            //0x26,0x04, //int 4
            //0x02,0x49,0x00,0x64,0x00, //columnname 2 chars ID
            writer.NewPackage();
            var columns = writer.ColumnsMetadata;
            writer.WriteByte(TdsEnums.SQLCOLMETADATA);
            writer.WriteInt16(columns.Length);
            for (var i = 0; i < columns.Length; i++)
                WriteColumn(writer, columns[i]);
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
                writer.WriteString(md.PartTableName.TableName);
            }
            writer.WriteByte((byte)md.Column.Length);
            writer.WriteString(md.Column);
        }

        public static void WriteBulkInt32(TdsPackageWriter writer, int? value)
        {
            writer.WriteByte(value == 0 ? 0 : 4);
            if (value != null)
                writer.WriteInt32((int)value);
        }
        public static void WriteBulkInt32(TdsPackageWriter writer, int value)
        {
            writer.WriteInt32(value);
        }
        private static readonly byte[] SLongDataHeader = { 0x10, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
        //internal void WriteBulkCopyValue(TdsPackageWriter writer, MetadataBulkCopy metadata, object value)
        //{
        //    var mt = TdsMetaType.TdsTypes[metadata.TdsType];

        //    if (value == null)
        //    {
        //        if (metadata.IsPlp)
        //            writer.WriteUInt64(TdsEnums.SQL_PLP_NULL);
        //        else if (mt.LenBytes == 2)
        //            writer.WriteInt16(TdsEnums.VARNULL);
        //        else
        //            writer.WriteByte(TdsEnums.FIXEDNULL);
        //        return;
        //    }
        //    if (metadata.IsTextOrImage)
        //        writer.WriteByteArray(SLongDataHeader, SLongDataHeader.Length);

        //    //                var dataLen = GetDataLen(value, metadata);
        //    //                if (metadata.IsPlp)
        //    //                    writer.WriteUInt64(TdsEnums.SQL_PLP_UNKNOWNLEN);
        //    //                else if (mt.LenBytes == 4) //variant
        //    //                    writer.WriteInt32(dataLen);
        //    //                else if (mt.LenBytes == 2)
        //    //                    writer.WriteInt16(dataLen);
        //    //                else if (mt.LenBytes == 1)
        //    //                    writer.WriteByte(dataLen);


        //    //                else if (metatype.SqlDbType != SqlDbType.Udt || metatype.IsLong)
        //    //                {
        //    //                    internalWriteTask = WriteValue(value, metatype, metadata.scale, ccb, ccbStringBytes, 0, stateObj, metadata.length, isDataFeed);
        //    //                    if (internalWriteTask == null && AsyncWrite) internalWriteTask = stateObj.WaitForAccumulatedWrites();
        //    //                    Debug.Assert(AsyncWrite || stateObj.WaitForAccumulatedWrites() == null, "Should not have accumulated writes when writing sync");
        //    //                }
        //    //                else
        //    //                {
        //    //                    WriteShort(ccb, stateObj);
        //    //                    internalWriteTask = stateObj.WriteByteArray((byte[])value, ccb, 0);
        //    //                }

        //    //#if DEBUG
        //    //                //In DEBUG mode, when SetAlwaysTaskOnWrite is true, we create a task. Allows us to verify async execution paths.
        //    //                if (AsyncWrite && internalWriteTask == null && SqlBulkCopy.SetAlwaysTaskOnWrite) internalWriteTask = Task.FromResult<object>(null);
        //    //#endif
        //    //                if (internalWriteTask != null) resultTask = WriteBulkCopyValueSetupContinuation(internalWriteTask, saveEncoding, saveCollation, saveCodePage, saveLCID);
        //    //            }
        //    //            finally
        //    //            {
        //    //                if (internalWriteTask == null)
        //    //                {
        //    //                    DefaultEncoding = saveEncoding;
        //    //                    _defaultCollation = saveCollation;
        //    //                    _defaultCodePage = saveCodePage;
        //    //                    DefaultLcid = saveLCID;
        //    //                }
        //    //            }

        //    //            return resultTask;
        //}

        //private int WriteDataLen(object o, MetadataBulkCopy metadata)
        //{
        //    switch (metadata.TdsType)
        //    {
        //        case TdsEnums.SQLBIGCHAR:
        //        case TdsEnums.SQLBIGVARCHAR:
        //        case TdsEnums.SQLTEXT:
        //            return metadata.Encoding.GetByteCount((string)o);
        //        case TdsEnums.SQLNCHAR:
        //        case TdsEnums.SQLNVARCHAR:
        //        case TdsEnums.SQLNTEXT:
        //            return ((string)o).Length;
        //        case TdsEnums.SQLBIGBINARY:
        //        case TdsEnums.SQLBIGVARBINARY:
        //        case TdsEnums.SQLIMAGE:
        //        case TdsEnums.SQLUDT:
        //            return ((byte[])o).Length;
        //        default:
        //            return metadata.Length;

        //    }
        //}

        private ulong GuidSize = 16;
        private string CreateParameter(MetadataBulkCopy mt)
        {
            var size = (mt.IsPlp ? "max" : mt.Length.ToString());
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
                case TdsEnums.SQLNVARCHAR: return $" NVarChar({ size})";
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


        public List<T> ExecuteQuery<T>(string text) where T : class, new()
        {

            _tdsPackage.Writer.SendExcuteBatch(text);
            _streamParser.ParseInput();
            if (_streamParser.Status == ParseStatus.Done)
                return null;
            var r = new List<T>();
            var rowReader = GetRowReader<T>(text);
            var columnReader = new TdsColumnReader(_tdsPackage.Reader);
            while (_streamParser.Status != ParseStatus.Done)
            {
                var result = rowReader(columnReader);
                _streamParser.ParseInput();
                r.Add(result);
            }
            return r;
        }



        public Func<TdsColumnReader, T> GetRowReader<T>(string key) where T : class, new()
        {
            return (Func<TdsColumnReader, T>)Readers.GetOrAdd(key, x => RowReader.GetComplexReader<T>(_tdsPackage.Reader));
        }

        public void Dispose()
        {
            // Oeps we are responsible for returning clean to the pool!! The pool should clone the connectionstate.
            // Fix later when this become complex
            if (Status == ParseStatus.Done) //Don't return connections to the pool that have problems
            {
                if (SqlMessages.Count > _messageCountAfterlogin)
                    SqlMessages.RemoveAt(_messageCountAfterlogin - 1);
                Tds.Return(_connectionString, this);
            }
        }
    }
}