using System;
using System.Globalization;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.SNI;
using Medella.TdsClient.TDS.Controller.Sspi;
using Medella.TdsClient.TDS.Messages.Client;
using Medella.TdsClient.TDS.Messages.Server;
using Medella.TdsClient.TDS.Messages.Server.Environment;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Package;
using SqlClient;
using SqlClient.TDS.Messages.Server;

namespace Medella.TdsClient.TDS
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

    public class TdsController
    {
        private static readonly byte[] ValidTokens =
        {
            TdsEnums.SQLERROR,
            TdsEnums.SQLINFO,
            TdsEnums.SQLLOGINACK,
            TdsEnums.SQLENVCHANGE,
            TdsEnums.SQLRETURNVALUE,
            TdsEnums.SQLRETURNSTATUS,
            TdsEnums.SQLCOLNAME,
            TdsEnums.SQLCOLFMT,
            TdsEnums.SQLCOLMETADATA,
            TdsEnums.SQLALTMETADATA,
            TdsEnums.SQLTABNAME,
            TdsEnums.SQLCOLINFO,
            TdsEnums.SQLORDER,
            TdsEnums.SQLALTROW,
            TdsEnums.SQLROW,
            TdsEnums.SQLNBCROW,
            TdsEnums.SQLDONE,
            TdsEnums.SQLDONEPROC,
            TdsEnums.SQLDONEINPROC,
            TdsEnums.SQLROWCRC,
            TdsEnums.SQLSECLEVEL,
            TdsEnums.SQLPROCID,
            TdsEnums.SQLOFFSET,
            TdsEnums.SQLSSPI,
            TdsEnums.SQLFEATUREEXTACK,
            TdsEnums.SQLSESSIONSTATE
        };

        private static readonly bool[] ValidTokensLookup = new bool[256];
        private readonly TdsPackageReader _reader;
        private readonly TdsPackageWriter _writer;
        private readonly SspiHelper _sspi;

        public ParseStatus Status;
        private bool _errorReceived;

        static TdsController()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            foreach (var token in ValidTokens)
                ValidTokensLookup[token] = true;
        }

        public TdsController(TdsPackageReader reader, TdsPackageWriter writer, SspiHelper sspi)
        {
            _reader = reader;
            _writer = writer;
            _sspi = sspi;
        }

        private string GetBytesString(string prefix, byte[] buffer, int offset, int length)
        {
            var sb = new StringBuilder($"{prefix}lentgh:{length,4:##0} ");
            sb.Append("data: ");
            for (var i = 0; i < length; i++)
                sb.Append($"{buffer[offset + i],2:X2} ");
            return sb.ToString();
        }

        public void ParseInput()
        {
            Status = ParseStatus.Unknown;
            while (_reader.ReadPackage() > 0)
            {
                _reader.GetBytesString("ParseInput: ");// GetBytesString("Invalid token:", _reader.ReadBuffer, _reader.GetReadPos(), _reader.PackageBytesLeft());
                var token = _reader.ReadByte();
                if (!ValidTokensLookup[token]) throw new Exception($"Invalid Token {token}");

                var tokenLength = _reader.GetTokenLength(token);
                switch (token)
                {
                    case TdsEnums.SQLERROR:
                    case TdsEnums.SQLINFO:
                        {
                            if (token == TdsEnums.SQLERROR) _errorReceived = true;
                            var error = _reader.SqlErrorAndInfo(token, tokenLength);
                            //_pendingInfoEvents.Add(error);
                            break;
                        }

                    case TdsEnums.SQLCOLINFO:
                        {
                            throw new NotImplementedException();
                            //if (null != _dataStream)
                            //{
                            //    var metaDataSet = TryProcessColInfo(_dataStream.MetaData, _dataStream);
                            //    _dataStream.TrySetMetaData(metaDataSet, false);
                            //    _dataStream.BrowseModeInfoConsumed = true;
                            //}
                            //else
                            //{
                            //    SkipBytes(tokenLength);
                            //}

                            //break;
                        }

                    case TdsEnums.SQLDONE:
                    case TdsEnums.SQLDONEPROC:
                    case TdsEnums.SQLDONEINPROC:
                        {
                            var isDone = _reader.SqlDone();
                            if (isDone)
                            {
                                Status = ParseStatus.Done;
                                return;
                            }
                            break;
                        }

                    case TdsEnums.SQLORDER:
                        {
                            throw new NotImplementedException();
                        }

                    case TdsEnums.SQLALTMETADATA:
                        {
                            throw new NotImplementedException();
                        }

                    case TdsEnums.SQLALTROW:
                        {
                            throw new NotImplementedException();
                        }

                    case TdsEnums.SQLENVCHANGE:
                        {
                            _reader.EnvChange(tokenLength);
                            // ENVCHANGE must be processed synchronously (since it can modify the state of many objects)
                            break;
                        }
                    case TdsEnums.SQLLOGINACK:
                        {
                            var serverInfo = _reader.LoginAck();
                            break;
                        }
                    case TdsEnums.SQLFEATUREEXTACK:
                        {
                            _reader.FeatureExtAck();
                            break;
                        }
                    case TdsEnums.SQLSESSIONSTATE:
                    {
                        _reader.GetBytes(tokenLength);
                        break;
                            throw new NotImplementedException();
                        }
                    case TdsEnums.SQLCOLMETADATA:
                        {
                            _reader.CurrentResultset.ColumnsMetadata = _reader.ColMetaData(tokenLength);
                            break;
                        }
                    case TdsEnums.SQLROW:
                        {
                            _reader.CurrentRow.IsNbcRow = false;
                            Status = ParseStatus.Row;
                            return;
                        }
                    case TdsEnums.SQLNBCROW:
                        {
                            _reader.SaveNbcBitmap();
                            Status = ParseStatus.Row;
                            return;
                        }
                    case TdsEnums.SQLRETURNSTATUS:
                        throw new NotImplementedException();
                    case TdsEnums.SQLRETURNVALUE:
                        {
                            throw new NotImplementedException();
                        }
                    case TdsEnums.SQLSSPI:
                        {
                            _reader.ParseToken(_sspi, tokenLength);
                            if (_sspi.ClientToken.Length == 0) break;
                            _writer.SendSspi(_sspi.ClientToken);
                            break;
                        }
                    case TdsEnums.SQLTABNAME:
                        {
                            throw new NotImplementedException(nameof(TdsEnums.SQLTABNAME));
                        }

                    default:
                        throw new Exception("Unhandled token:  " + token.ToString(CultureInfo.InvariantCulture));
                }
            }
            Status = ParseStatus.Done;
            return;
        }

        public static void ReadRow()
        {
        }

        public object ReadColumn(int i)
        {
            // if we still have bytes left from the previous blob read, clear the wire and reset
            var columnMetaData = _reader.CurrentResultset.ColumnsMetadata[i];
            var dataLength = _reader.ReadColumnHeader(i);
            var value = dataLength == null
                ? null
                : ReadColumnValue(_reader, columnMetaData, (ulong)dataLength);
            if (i == _reader.CurrentResultset.ColumnsMetadata.Length - 1) Status = ParseStatus.AfterLastColumn;
            return value;
        }

        public object ReadColumnValue(TdsPackageReader reader, ColumnMetadata md, ulong length)
        {
            var isPlp = md.IsPlp;
            var tdsType = md.TdsType;

            if (isPlp) length = int.MaxValue;
            switch (tdsType)
            {
                case TdsEnums.SQLDECIMALN:
                case TdsEnums.SQLNUMERICN:
                    return reader.ReadSqlDecimal((int)length, md.Scale);

                case TdsEnums.SQLUDT:
                case TdsEnums.SQLBINARY:
                case TdsEnums.SQLBIGBINARY:
                case TdsEnums.SQLBIGVARBINARY:
                case TdsEnums.SQLVARBINARY:
                case TdsEnums.SQLIMAGE:
                    // If varbinary(max), we only read the first chunk here, expecting the caller to read the rest
                    return isPlp
                        ? reader.ReadPlpBlobBytes(length)
                        : reader.ReadByteArray(new byte[length], 0, (int)length);
                case TdsEnums.SQLCHAR:
                case TdsEnums.SQLBIGCHAR:
                case TdsEnums.SQLVARCHAR:
                case TdsEnums.SQLBIGVARCHAR:
                case TdsEnums.SQLTEXT:
                    return isPlp
                        ? reader.ReadPlpString(md.Encoding, length)
                        : reader.ReadString(md.Encoding, (int)length);
                case TdsEnums.SQLNCHAR:
                case TdsEnums.SQLNVARCHAR:
                case TdsEnums.SQLNTEXT:
                    return isPlp
                        ? reader.ReadPlpUnicodeChars(length)
                        : reader.ReadUnicodeChars((int)length);

                case TdsEnums.SQLXMLTYPE:
                    return reader.ReadUnicodeChars((int)length);

                case TdsEnums.SQLDATE:
                    return reader.ReadSqlDate();
                case TdsEnums.SQLTIME:
                    return reader.ReadSqlTime((int)length, md.Scale);
                case TdsEnums.SQLDATETIME2:
                    return reader.ReadSqlDateTime((int)length, md.Scale);
                case TdsEnums.SQLDATETIMEOFFSET:
                    return reader.ReadSqlDateTimeOffset((int)length, md.Scale);
                //if (!TryReadSqlDateTime(value, tdsType, length, md.scale, stateObj)) return false;

                case TdsEnums.SQLBIT:
                case TdsEnums.SQLBITN:
                    return reader.ReadByte() != 0;

                case TdsEnums.SQLINTN:
                    if (length == 1)
                        goto case TdsEnums.SQLINT1;
                    else if (length == 2)
                        goto case TdsEnums.SQLINT2;
                    else if (length == 4)
                        goto case TdsEnums.SQLINT4;
                    else
                        goto case TdsEnums.SQLINT8;

                case TdsEnums.SQLINT1:
                    return reader.ReadByte();

                case TdsEnums.SQLINT2:
                    return reader.ReadInt16();

                case TdsEnums.SQLINT4:
                    return reader.ReadInt32();

                case TdsEnums.SQLINT8:
                    return reader.ReadInt64();

                case TdsEnums.SQLFLTN:
                    if (length == 4)
                        goto case TdsEnums.SQLFLT4;
                    else
                        goto case TdsEnums.SQLFLT8;

                case TdsEnums.SQLFLT4:
                    return reader.ReadType<float>((int)length);

                case TdsEnums.SQLFLT8:
                    return reader.ReadType<double>((int)length);

                case TdsEnums.SQLMONEYN:
                    if (length == 4)
                        goto case TdsEnums.SQLMONEY4;
                    else
                        goto case TdsEnums.SQLMONEY;

                case TdsEnums.SQLMONEY:
                    return reader.ReadSqlMoney((int)length);

                case TdsEnums.SQLMONEY4:
                    return reader.ReadSqlMoney((int)length);

                case TdsEnums.SQLDATETIMN:
                    if (length == 4)
                        goto case TdsEnums.SQLDATETIM4;
                    else
                        goto case TdsEnums.SQLDATETIME;

                case TdsEnums.SQLDATETIM4:
                    return reader.ReadSqlDateTime((int)length);

                case TdsEnums.SQLDATETIME:
                    return reader.ReadSqlDateTime((int)length);

                case TdsEnums.SQLUNIQUEID:
                    return reader.ReadGuid();
                case TdsEnums.SQLVARIANT:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
                    //if (!TryReadSqlValueInternal(value, tdsType, length, stateObj)) return false;
            }
        }

        public void SkipColumns()
        {
            throw new NotImplementedException();
        }
    }
}