using System;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Messages.Client;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Package;

namespace Medella.TdsClient.TDS.Writer
{
    public class TdsColumnWriter
    {
        private static readonly byte[] TextOrImageHeader = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
        private readonly TdsPackageWriter _writer;
        public readonly MetadataBulkCopy[] MetaData;

        public TdsColumnWriter(TdsPackageWriter writer)
        {
            _writer = writer;
            MetaData = writer.ColumnsMetadata;
        }

        public void WriteNullableSqlBit(bool? value, int index)
        {
            _writer.WriteByte(value == null ? 0 : 1);
            if (value != null)
                WriteSqlBit((bool)value, index);
        }

        public void WriteSqlBit(bool value, int index)
        {
            _writer.WriteByte(value ? 1 : 0);
        }

        public void WriteSqlByte(byte value, int index)
        {
            _writer.WriteByte(value);
        }


        public void WriteSqlInt16(short value, int index)
        {
            _writer.WriteInt16(value);
        }

        public void WriteNullableSqlInt(long? value, int index)
        {
            var len = MetaData[index].Length;
            _writer.WriteByte(value == null ? 0 : len);
            if (value == null) return;

            if (len == 1) WriteSqlByte((byte)value, index);
            if (len == 2) WriteSqlInt16((short)value, index);
            if (len == 4) WriteSqlInt32((int)value, index);
            if (len == 8) WriteSqlInt64((long)value, index);
        }
        public void WriteNullableSqlByte(byte? value, int index)
        {
            _writer.WriteByte(value == null ? 0 : 1);
            if (value == null) return;
            WriteSqlByte((byte)value, index);
        }
        public void WriteNullableSqlInt16(short? value, int index)
        {
            _writer.WriteByte(value == null ? 0 : 2);
            if (value == null) return;

            WriteSqlInt16((short)value, index);
        }
        public void WriteNullableSqlInt32(int? value, int index)
        {
            _writer.WriteByte(value == null ? 0 : 4);
            if (value == null) return;

            WriteSqlInt32((int)value, index);
        }
        public void WriteNullableSqlInt64(long? value, int index)
        {
            _writer.WriteByte(value == null ? 0 : 8);
            if (value == null) return;

            WriteSqlInt64((long)value, index);
        }


        public void WriteSqlInt32(int value, int index)
        {
            _writer.WriteInt32(value);
        }

        public void WriteSqlInt64(long value, int index)
        {
            _writer.WriteInt64(value);
        }

        public void WriteSqlMoney4(decimal value, int index)
        {
            _writer.WriteInt32((int)(value * 10000));
        }
        public void WriteNullableSqlMoneyN(decimal? value, int index)
        {
            var len = MetaData[index].Length;
            if (len == 4) WriteNullableSqlMoney4(value, index);
            if (len == 8) WriteNullableSqlMoney(value, index);
        }
        public void WriteNullableSqlMoney4(decimal? value, int index)
        {
            _writer.WriteByte(value == null ? 0 : 4);
            if (value == null) return;
            WriteSqlMoney4((decimal)value, index);
        }
        public void WriteNullableSqlMoney(decimal? value, int index)
        {
            _writer.WriteByte(value == null ? 0 : 8);
            if (value == null) return;
            WriteSqlMoney((decimal)value, index);
        }

        public void WriteSqlMoney(decimal value, int index)
        {
            var v = (long)(value * 10000);
            _writer.WriteUInt32((uint)(v >> 0x20));
            _writer.WriteUInt32((uint)(v & 0xFFFF_FFFF));
        }

        public void WriteNullableSqlFloat(float? value, int index)
        {
            _writer.WriteByte(value == null ? 0 : 4);
            if (value != null)
                _writer.WriteFloat((float)value);
        }

        public void WriteSqlFloat(float value, int index)
        {
            _writer.WriteFloat(value);
        }

        public void WriteNullableSqlDouble(double? value, int index)
        {
            _writer.WriteByte(value == null ? 0 : 8);
            if (value == null) return;
            _writer.WriteDouble((double)value);
        }

        public void WriteSqlDouble(double value, int index)
        {
            _writer.WriteDouble(value);
        }
        internal static readonly long[] DecimalToScale =
        {
            1,//0
            10,//1
            100,//2
            1000,//3
            10000,//4
            100000,//5
            1000000,//6
            10000000,//7
            100000000,//8
            1000000000,//9
            10000000000,//10
            100000000000,//11
            1000000000000,//12
            10000000000000,//13
            100000000000000,//14
            1000000000000000,//15
        };

        public void WriteNullableSqlDecimal(decimal? value, int index)
        {
            var p = MetaData[index].Precision;
            var len = p <= 9 ? 5
                : p <= 19 ? 9
                : p <= 28 ? 13
                : 17;
            _writer.WriteByte(value == null ? 0 : len);
            if (value == null) return;
            var scale = MetaData[index].Scale;
            WriteSqlDecimal((decimal)value, len, scale);
        }

        private void WriteSqlDecimal(decimal value, int len, byte toScale)
        {
            var fromScale = (byte)(decimal.GetBits(value)[3] >> 16);
            value = CorrectScale(value, fromScale, toScale);
            _writer.WriteSqlDecimal(value, len);
        }

        private static decimal CorrectScale(decimal value, byte fromScale, byte toScale)
        {
            var corr = toScale - fromScale;
            if (corr < 0)
                return decimal.Round(value, toScale);
            if (corr <= 15)
                return value * DecimalToScale[corr];
            value = value * DecimalToScale[15];
            return value * DecimalToScale[corr - 15];
        }

        public void WriteNullableSqlDate(DateTime? value, int index)
        {
            _writer.WriteByte(value == null ? 0 : 3);
            if (value == null) return;
            _writer.WriteDate((DateTime)value);
        }

        public void WriteNullableSqlTime(TimeSpan? value, int index)
        {
            var scale = MetaData[index].Scale;
            var len = scale <= 2 ? 3 : scale <= 4 ? 4 : 5;
            _writer.WriteByte(value == null ? 0 : len);
            if (value == null) return;
            WriteSqlTime((TimeSpan)value, scale);
        }

        private void WriteSqlTime(TimeSpan value, byte scale)
        {
            var len = scale <= 2 ? 3 : scale <= 4 ? 4 : 5;
            var v = (ulong)(value.Ticks / TdsEnums.TICKS_FROM_SCALE[scale]);
            _writer.WriteByte((byte)v);
            _writer.WriteByte((byte)(v >> 8));
            _writer.WriteByte((byte)(v >> 16));
            if (len > 3) _writer.WriteByte((byte)(v >> 24));
            if (len > 4) _writer.WriteByte((byte)(v >> 32));
        }

        public void WriteNullableSqlDateTime2(DateTime? value, int index)
        {
            var scale = MetaData[index].Scale;
            var len = scale <= 2 ? 6
                : scale <= 4 ? 7
                : 8;
            _writer.WriteByte(value == null ? 0 : len);
            if (value == null) return;
            WriteSqlTime(value.Value.TimeOfDay, scale);
            _writer.WriteDate((DateTime)value);
        }

        public void WriteNullableSqlDateTimeOffset(DateTimeOffset? value, int index)
        {
            var scale = MetaData[index].Scale;
            var len = scale <= 2 ? 8
                : scale <= 4 ? 9
                : 10;
            _writer.WriteByte(value == null ? 0 : len);
            if (value == null) return;
            WriteSqlDateTimeOffset((DateTimeOffset)value, scale);
        }

        private void WriteSqlDateTimeOffset(DateTimeOffset value, byte scale)
        {
            value = value.Subtract(value.Offset);
            WriteSqlTime(value.TimeOfDay, scale);
            _writer.WriteDate(value.DateTime);
            _writer.WriteInt16(value.Offset.Minutes);
        }

        public static readonly DateTime BaseDate1900 = new DateTime(1900, 1, 1);
        public void WriteSqlDateTime4(DateTime value, int index)
        {
            var datepart = (ushort)value.Subtract(BaseDate1900).Days;
            var timepart = (ushort)value.TimeOfDay.TotalMinutes;
            _writer.WriteInt16(datepart);
            _writer.WriteInt16(timepart);
        }
        public void WriteSqlDateTime(DateTime value, int index)
        {
            var datepart = (int)value.Subtract(BaseDate1900).Days;
            var timepart = (int)value.TimeOfDay.TotalSeconds * 300;
            _writer.WriteInt32(datepart);
            _writer.WriteInt32(timepart);
        }
        public void WriteNullableSqlDateTime(DateTime? value, int index)
        {
            var len = MetaData[index].TdsType == TdsEnums.SQLDATETIM4 ? 4 : 8;

            _writer.WriteByte(value == null ? 0 : len);
            if (value == null) return;
            if (len == 4)
                WriteSqlDateTime4((DateTime)value, index);
            else
                WriteSqlDateTime((DateTime)value, index);
        }

        public void WriteNullableSqlUniqueId(Guid? value, int index)
        {
            _writer.WriteByte(value == null ? 0 : 16);
            if (value == null) return;
            _writer.WriteByteArray(((Guid)value).ToByteArray());
        }
        //0xD1,row
        //0xFE,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF, Unknown length
        //0x01,0x00,0x00,0x00, chunk
        //0x31,
        //0x00,0x00,0x00,0x00, terminator
        public void WriteNullableSqlBinary(byte[] value, int index)
        {
            var md = MetaData[index];
            if (md.IsPlp)
                WriteNullableSqlPlpBinary(value);
            else if (md.IsTextOrImage)
                WriteNullableSqlImage(value);
            else
                WriteNullableSqlBinary(value);
        }

        private void WriteNullableSqlBinary(byte[] value)
        {
            _writer.WriteInt16(value?.Length ?? TdsEnums.VARNULL);
            if (value == null) return;
            _writer.WriteByteArray(value);
        }

        private void WriteNullableSqlImage(byte[] value)
        {
            _writer.WriteByte(value == null ? 0 : 0x10);
            if (value == null) return;
            _writer.WriteByteArray(TextOrImageHeader);
            _writer.WriteInt32(value.Length);
            _writer.WriteByteArray(value);
        }

        private void WriteNullableSqlPlpBinary(byte[] value)
        {
            _writer.WriteUInt64(value == null ? TdsEnums.SQL_PLP_NULL : TdsEnums.SQL_PLP_UNKNOWNLEN);
            if (value == null) return;
            _writer.WriteInt32(value.Length); //write in chunks
            _writer.WriteByteArray(value);
            _writer.WriteInt32(0); //chunks terminate
        }

        public void WriteNullableSqlString(string value, int index)
        {
            if (value == null)
            {
                WriteNullableSqlBinary(null, index);
                return;
            }
            var md = MetaData[index];
            if (md.NonUniCode)
                WriteNullableSqlBinary(md.Encoding.GetBytes(value), index);
            else
                WriteNullableSqlBinary(Encoding.Unicode.GetBytes(value), index);
        }
        public void WriteNullableSqlVariant(object value, int index)
        {
            if (value == null)
            {
                _writer.WriteInt32(0);
                return;
            }
            //
            // now Write the value
            //
            switch (value)
            {
                case bool v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 1);
                    _writer.WriteByte(TdsEnums.SQLBIT);
                    _writer.WriteByte(0);
                    _writer.WriteByte(v ? 1 : 0);
                    return;
                case byte v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 1);
                    _writer.WriteByte(TdsEnums.SQLINT1);
                    _writer.WriteByte(0);
                    _writer.WriteByte(v);
                    return;
                case short v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 2);
                    _writer.WriteByte(TdsEnums.SQLINT2);
                    _writer.WriteByte(0);
                    _writer.WriteInt16(v);
                    return;
                case int v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 4);
                    _writer.WriteByte(TdsEnums.SQLINT4);
                    _writer.WriteByte(0);
                    _writer.WriteInt32(v);
                    return;
                case long v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 8);
                    _writer.WriteByte(TdsEnums.SQLINT8);
                    _writer.WriteByte(0);
                    _writer.WriteInt64(v);
                    return;
                case float v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 4);
                    _writer.WriteByte(TdsEnums.SQLFLT4);
                    _writer.WriteByte(0);
                    _writer.WriteFloat(v);
                    return;
                case double v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 8);
                    _writer.WriteByte(TdsEnums.SQLFLT8);
                    _writer.WriteByte(0);
                    _writer.WriteDouble(v);
                    return;
                case DateTime v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 8);
                    _writer.WriteByte(TdsEnums.SQLDATETIME);
                    _writer.WriteByte(0);
                    _writer.WriteDateTime(v);
                    return;
                case Guid v:
                    _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 16);
                    _writer.WriteByte(TdsEnums.SQLUNIQUEID);
                    _writer.WriteByte(0);
                    _writer.WriteSqlGuid(v);
                    return;
                case decimal v:
                    {
                        _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 2 + 13);
                        _writer.WriteByte(TdsEnums.SQLDECIMALN);
                        _writer.WriteByte(2);
                        _writer.WriteByte(28);
                        var scale = (byte)(decimal.GetBits(v)[3] >> 16);
                        _writer.WriteByte(scale);
                        _writer.WriteSqlDecimal(v, 13);
                        return;
                    }
                case byte[] v:
                    {
                        _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 2 + v.Length);
                        _writer.WriteByte(TdsEnums.SQLBIGBINARY);
                        _writer.WriteByte(2);
                        _writer.WriteInt16(v.Length);
                        _writer.WriteByteArray(v);
                        return;
                    }
                case string v:
                    {
                        var collation = _writer.ColumnsMetadata[index].Collation;
                        var encoding = _writer.ColumnsMetadata[index].Encoding;
                        var bytes = encoding.GetBytes(v);
                        _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 5 + 2 + bytes.Length);
                        _writer.WriteByte(TdsEnums.SQLBIGVARCHAR);
                        _writer.WriteByte(7);
                        _writer.WriteUInt32(collation.Info);
                        _writer.WriteByte(collation.SortId);
                        WriteNullableSqlBinary(bytes);
                        return;
                    }
                case TimeSpan v:
                    {
                        const byte scale = 7;
                        const int len = 5;
                        _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 1 + len);
                        _writer.WriteByte(TdsEnums.SQLTIME);
                        _writer.WriteByte(1);
                        _writer.WriteByte(scale);
                        WriteSqlTime(v, scale);
                        return;
                    }
                case DateTimeOffset v:
                    {
                        const byte scale = 7;
                        const int len = 10;
                        _writer.WriteInt32(TdsEnums.SQLVARIANT_SIZE + 1 + len);
                        _writer.WriteByte(TdsEnums.SQLDATETIMEOFFSET);
                        _writer.WriteByte(1);
                        _writer.WriteByte(scale);
                        WriteSqlDateTimeOffset(v, scale);
                        return;
                    }
            }
            throw new Exception("Unsupported variant object");
        }
    }
}