using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Messages.Server;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Reader.StringHelpers;

namespace Medella.TdsClient.TDS.Reader
{
    public class TdsColumnReader
    {
        private readonly TdsPackageReader _reader;
        public readonly ColumnsMetadata MetaData;

        public TdsColumnReader(TdsPackageReader reader)
        {
            _reader = reader;
            MetaData = reader.CurrentResultset.ColumnsMetadata;
        }

        public decimal? ReadDecimal(int index)
        {
            var length = _reader.ReadColumnHeader(index);
            return length == null
                ? (decimal?)null
                : _reader.ReadSqlDecimal((int)length, MetaData[index].Scale);
        }

        public byte[] ReadBinary(int index)
        {
            var isPlp = _reader.CurrentResultset.ColumnsMetadata[index].IsPlp;
            var length = _reader.ReadColumnHeader(index);
            return length == null
                ? null
                : isPlp
                    ? _reader.ReadPlpBlobBytes((ulong)length)
                    : _reader.ReadByteArray(new byte[(int)length], 0, (int)length);
        }

        public string ReadString(int index)
        {
            var md = _reader.CurrentResultset.ColumnsMetadata[index];
            var isPlp = md.IsPlp;
            var length = _reader.ReadColumnHeader(index);
            return length == null
                ? null
                : isPlp
                    ? _reader.ReadPlpString(md.Encoding, (ulong)length)
                    : _reader.ReadString(md.Encoding, (int)length);
        }

        public string ReadUnicodeString(int index)
        {
            var md = _reader.CurrentResultset.ColumnsMetadata[index];
            var isPlp = md.IsPlp;
            var length = _reader.ReadColumnHeader(index);
            return length == null
                ? null
                : isPlp
                    ? _reader.ReadPlpUnicodeChars((ulong)length)
                    : _reader.ReadUnicodeChars((int)length);
        }

        public DateTime? ReadSqlDate(int index)
        {
            var length = _reader.ReadColumnHeader(index);
            return length == null
                ? (DateTime?)null
                : _reader.ReadSqlDate();
        }

        public TimeSpan? ReadSqlTime(int index)
        {
            var scale = _reader.CurrentResultset.ColumnsMetadata[index].Scale;
            var length = _reader.ReadColumnHeader(index);
            return length == null ? (TimeSpan?)null : _reader.ReadSqlTime((int)length, scale);
        }

        public DateTime? ReadSqlDateTime2(int index)
        {
            var scale = _reader.CurrentResultset.ColumnsMetadata[index].Scale;
            var length = _reader.ReadColumnHeader(index);
            return length == null ? (DateTime?)null : _reader.ReadSqlDateTime((int)length, scale);
        }

        public DateTime? ReadSqlDateTime(int index)
        {
            var length = _reader.ReadColumnHeader(index);
            return length == null ? (DateTime?)null : _reader.ReadSqlDateTime((int)length);
        }

        public DateTimeOffset? ReadSqlDateTimeOffset(int index)
        {
            var scale = _reader.CurrentResultset.ColumnsMetadata[index].Scale;
            var length = _reader.ReadColumnHeader(index);
            return length == null ? (DateTimeOffset?)null : _reader.ReadSqlDateTimeOffset((int)length, scale);
        }

        public bool? ReadSqlBit(int index)
        {
            var length = _reader.ReadColumnHeader(index);
            return length == null ? (bool?)null : _reader.ReadByte() != 0;
        }

        public long? ReadSqlIntN(int index)
        {
            var length = _reader.ReadColumnHeader(index);
            switch (length)
            {
                case 1: return _reader.ReadByte();
                case 2: return _reader.ReadInt16();
                case 4: return _reader.ReadInt32();
                case 8: return _reader.ReadInt64();
            }
            return null;
        }
        public byte? ReadSqlByte(int index)
        {
            var length = _reader.ReadColumnHeader(index);
            return length == null ? (byte?)null : _reader.ReadByte();
        }

        public short? ReadSqlInt16(int index)
        {
            var length = _reader.ReadColumnHeader(index);
            return length == null ? (short?)null : _reader.ReadInt16();
        }

        public int? ReadSqlInt32(int index)
        {
            var length = _reader.ReadColumnHeader(index);
            return length == null ? (int?)null : _reader.ReadInt32();
        }

        public long? ReadSqlInt64(int index)
        {
            var length = _reader.ReadColumnHeader(index);
            return length == null ? (long?)null : _reader.ReadInt64();
        }

        public double? ReadSqlFloatN(int index)
        {
            var length = _reader.ReadColumnHeader(index);
            switch (length)
            {
                case 4: return _reader.ReadType<float>(4);
                case 8: return _reader.ReadType<double>(8);
            }
            return null; 
        }
        public float? ReadSqlFloat(int index)
        {
            var length = _reader.ReadColumnHeader(index);
            return length == null ? (float?)null : _reader.ReadType<float>((int)length);
        }


        public double? ReadSqlDouble(int index)
        {
            var length = _reader.ReadColumnHeader(index);
            return length == null ? (double?)null : _reader.ReadType<double>((int)length);
        }

        public decimal? ReadSqlMoney(int index)
        {
            var length = _reader.ReadColumnHeader(index);
            return length == null ? (decimal?)null : _reader.ReadSqlMoney((int)length);
        }

        public Guid? ReadSqlGuid(int index)
        {
            var length = _reader.ReadColumnHeader(index);
            return length == null ? (Guid?)null : _reader.ReadGuid();
        }
        public object ReadSqlVariant(int index)
        {
            var lenTotal = (int?)_reader.ReadColumnHeader(index);
            // get the SQLVariant type
            if (lenTotal == null)
            {
                return null;
                _reader.GetBytesString("variantnull:");
            }
            var type = _reader.ReadByte();
            // read cbPropBytes
            var cbPropsActual = _reader.ReadByte();

            var lenConsumed = TdsEnums.SQLVARIANT_SIZE + cbPropsActual; // type, count of propBytes, and actual propBytes
            var lenData = (int)lenTotal - lenConsumed; // length of actual data

            // read known properties and skip unknown properties

            //
            // now read the value
            //
            switch (type)
            {
                case TdsEnums.SQLBIT:
                    return _reader.ReadByte() != 0;
                case TdsEnums.SQLINT1:
                    return _reader.ReadByte();
                case TdsEnums.SQLINT2:
                    return _reader.ReadInt16();
                case TdsEnums.SQLINT4:
                    return _reader.ReadInt32();
                case TdsEnums.SQLINT8:
                    return _reader.ReadInt64();
                case TdsEnums.SQLFLT4:
                    return _reader.ReadType<float>(4);
                case TdsEnums.SQLFLT8:
                    return _reader.ReadType<double>(8);
                case TdsEnums.SQLMONEY:
                    return _reader.ReadSqlMoney(8);
                case TdsEnums.SQLMONEY4:
                    return _reader.ReadSqlMoney(4);
                case TdsEnums.SQLDATETIM4:
                    return _reader.ReadSqlDateTime(4);
                case TdsEnums.SQLDATETIME:
                    return _reader.ReadSqlDateTime(8);
                case TdsEnums.SQLUNIQUEID:
                    return _reader.ReadGuid();
                case TdsEnums.SQLDECIMALN:
                case TdsEnums.SQLNUMERICN:
                {
                    var precision = _reader.ReadByte();
                    var scale = _reader.ReadByte();
                    return _reader.ReadSqlDecimal(lenData, scale);
                }

                case TdsEnums.SQLBIGBINARY:
                case TdsEnums.SQLBIGVARBINARY:
                {
                    var lenmax = _reader.ReadUInt16();
                    return _reader.ReadByteArray(new byte[lenData], 0, lenData);
                }
                case TdsEnums.SQLBIGCHAR:
                case TdsEnums.SQLBIGVARCHAR:
                {
                    var collation = _reader.ReadCollation();
                    var lenmax = _reader.ReadUInt16();
                    var encoding = Encoding.GetEncoding(collation.GetCodePage());
                    return _reader.ReadString(encoding, lenData);
                }
                case TdsEnums.SQLNCHAR:
                case TdsEnums.SQLNVARCHAR:
                {
                    var collation = _reader.ReadCollation();
                    var lenmax = _reader.ReadUInt16();
                    return _reader.ReadUnicodeChars(lenData);
                }
                case TdsEnums.SQLDATE:
                    return _reader.ReadSqlDate();
                case TdsEnums.SQLTIME:
                {
                    var scale = _reader.ReadByte();
                    return _reader.ReadSqlTime(lenData, scale);
                }
                case TdsEnums.SQLDATETIME2:
                {
                    var scale = _reader.ReadByte();
                    return _reader.ReadSqlDateTime(lenData, scale);
                }
                case TdsEnums.SQLDATETIMEOFFSET:
                {
                    var scale = _reader.ReadByte();
                    return _reader.ReadSqlDateTimeOffset(lenData, scale);
                }
            }

            return null;
        }
    }
}