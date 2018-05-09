using System;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Messages.Server.Internal;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Reader;
using Medella.TdsClient.TDS.Reader.StringHelpers;
using Xunit;

namespace TdsClientTests
{
    public class WriteReadValueVariantTest
    {

    }

    public class WriteReadValueTest
    {
        [Fact]
        public void Variant()
        {
            var text = new string('*', 10);
            var arr = new byte[10];
            TestIntN(new SqlVariant((bool)true), TdsEnums.SQLVARIANT, true);
            TestIntN(new SqlVariant((byte)1), TdsEnums.SQLVARIANT, true);
            TestIntN(new SqlVariant((short)1), TdsEnums.SQLVARIANT, true);
            TestIntN(new SqlVariant((int)1), TdsEnums.SQLVARIANT, true);
            TestIntN(new SqlVariant((long)1), TdsEnums.SQLVARIANT, true);
            TestIntN(new SqlVariant((float)1), TdsEnums.SQLVARIANT, true);
            TestIntN(new SqlVariant((double)1), TdsEnums.SQLVARIANT, true);
            TestIntN(new SqlVariant(new DateTime(2018, 1, 2, 0, 0, 0)), TdsEnums.SQLVARIANT, true);
            TestIntN(new SqlVariant(1M), TdsEnums.SQLVARIANT, true);
            TestIntN(new SqlVariant(Guid.NewGuid()), TdsEnums.SQLVARIANT, true);
            TestIntN(new SqlVariant(arr), TdsEnums.SQLVARIANT, true);
            TestIntN(new SqlVariant(text), TdsEnums.SQLVARIANT, true);
            TestIntN(new SqlVariant(new DateTimeOffset(2018, 1, 2, 3, 4, 5, new TimeSpan(5, 0, 0))), TdsEnums.SQLVARIANT, true);
            TestIntN(new SqlVariant(new TimeSpan(1, 2, 3)), TdsEnums.SQLVARIANT, true);
        }
        [Fact]
        public void BinObjects()
        {
            var arr = new byte[10];
            TestIntN(arr, TdsEnums.SQLBIGBINARY, true);
            TestIntN(arr, TdsEnums.SQLBIGVARBINARY, true);
            TestIntN(arr, TdsEnums.SQLBIGVARBINARY, true, 0, 0, true);
            TestIntN((SqlImage)arr, TdsEnums.SQLIMAGE, true);
        }
        [Fact]
        public void StringObjects()
        {
            var text = new string('*', 10);
            TestIntN((SqlUnicode)text, TdsEnums.SQLNCHAR, true);
            TestIntN(text, TdsEnums.SQLBIGCHAR, true);
            TestIntN(text, TdsEnums.SQLBIGVARCHAR, true);
            TestIntN(text, TdsEnums.SQLBIGVARCHAR, true, 0, 0, true);
            TestIntN((SqlUnicode)text, TdsEnums.SQLNVARCHAR, true);
            TestIntN((SqlUnicode)text, TdsEnums.SQLNVARCHAR, true, 0, 0, true);
            TestIntN((SqlUnicode)text, TdsEnums.SQLNTEXT, true);
            TestIntN((SqlUnicode)text, TdsEnums.SQLNTEXT, true, 0, 0, true);
            TestIntN(text, TdsEnums.SQLTEXT, true);
            TestIntN(text, TdsEnums.SQLTEXT, true, 0, 0, true);
            TestIntN((SqlXml)"<Data><DepartmentID>x</DepartmentID></Data>", TdsEnums.SQLXMLTYPE, true, 0, 0, true);
        }

        [Fact]
        public void SimpleObjectsTypes()
        {
            TestIntN((Money4)4.0001M, TdsEnums.SQLMONEYN, true);
            TestIntN((Money4)4.0001M, TdsEnums.SQLMONEY4, false);
            TestIntN((Money)922_337_203_685_477.5807M, TdsEnums.SQLMONEYN, true);
            TestIntN((Money)922_337_203_685_477.5807M, TdsEnums.SQLMONEY, false);
            TestIntN((SqlDate)new DateTime(2018, 1, 2, 0, 0, 0), TdsEnums.SQLDATE, true);
            TestIntN(new TimeSpan(1, 2, 3), TdsEnums.SQLTIME, true);
            TestIntN(new TimeSpan(1, 2, 3), TdsEnums.SQLTIME, true, 0, 7);
            TestIntN((SqlDateTime2)new DateTime(2018, 1, 2, 3, 4, 5), TdsEnums.SQLDATETIME2, true, 0, 7);
            TestIntN((SqlDateTime2)new DateTime(2018, 1, 2, 3, 4, 5), TdsEnums.SQLDATETIME2, true, 0, 0);
            TestIntN(new DateTimeOffset(2018, 1, 2, 3, 4, 5, new TimeSpan(5, 0, 0)), TdsEnums.SQLDATETIMEOFFSET, true, 0, 0);
            TestIntN((SqlDateTime4)new DateTime(2018, 1, 2, 3, 4, 0), TdsEnums.SQLDATETIM4, false, 0, 0);
            TestIntN((SqlDateTime4)new DateTime(2018, 1, 2, 3, 4, 0), TdsEnums.SQLDATETIMN, true, 0, 0);
            TestIntN(new DateTime(2018, 1, 2, 3, 4, 5), TdsEnums.SQLDATETIME, false, 0, 0);
            TestIntN(new DateTime(2018, 1, 2, 3, 4, 5), TdsEnums.SQLDATETIMN, true, 0, 0);
            TestIntN(1M, TdsEnums.SQLDECIMALN, true, 28, 28);
            TestIntN(Guid.NewGuid(), TdsEnums.SQLUNIQUEID, true, 28, 28);
        }

        [Theory]
        [InlineData(true, TdsEnums.SQLBITN, true)]
        [InlineData(true, TdsEnums.SQLBIT, false)]
        [InlineData(byte.MaxValue, TdsEnums.SQLINTN, true)]
        [InlineData(byte.MaxValue, TdsEnums.SQLINT1, false)]
        [InlineData(short.MaxValue, TdsEnums.SQLINTN, true)]
        [InlineData(short.MaxValue, TdsEnums.SQLINT2, false)]
        [InlineData(int.MaxValue, TdsEnums.SQLINTN, true)]
        [InlineData(int.MaxValue, TdsEnums.SQLINT4, false)]
        [InlineData(long.MaxValue, TdsEnums.SQLINTN, true)]
        [InlineData(long.MaxValue, TdsEnums.SQLINT8, false)]
        [InlineData(float.MaxValue, TdsEnums.SQLFLTN, false)]
        [InlineData(float.MaxValue, TdsEnums.SQLFLT4, false)]
        [InlineData(double.MaxValue, TdsEnums.SQLFLTN, false)]
        [InlineData(double.MaxValue, TdsEnums.SQLFLT8, false)]
        private void TestIntN(object value, int tdsType, bool nulltest, byte precision = 0, byte scale = 0, bool isPlp = false)
        {
            var stream = new TestStream();
            var writer = new TdsPackageWriter(stream);
            var reader = new TdsPackageReader(stream);
            SetupColMetaData(reader, writer, tdsType, precision, scale, isPlp);
            var columwriter = new TdsColumnWriter(writer);
            var columnReader = new TdsColumnReader(reader);

            object result;
            if (nulltest)
            {
                writer.NewPackage();
                ObjectWriter(columwriter, tdsType, value, nulltest);
                writer.FlushBuffer();
                result = ObjectReader(columnReader, value);
                Assert.Null(result);
            }

            writer.NewPackage();
            ObjectWriter(columwriter, tdsType, value, false);
            writer.FlushBuffer();
            result = ObjectReader(columnReader, value);
            switch (value)
            {
                case Money v:
                    Assert.Equal((decimal)v, result);
                    break;
                case Money4 v:
                    Assert.Equal((decimal)v, result);
                    break;
                case SqlDate v:
                    Assert.Equal((DateTime)v, result);
                    break;
                case SqlDateTime2 v:
                    Assert.Equal((DateTime)v, result);
                    break;
                case SqlDateTime4 v:
                    Assert.Equal((DateTime)v, result);
                    break;
                case SqlImage v:
                    Assert.Equal((byte[])v, result);
                    break;
                case SqlUnicode v:
                    Assert.Equal((string)v, result);
                    break;
                case SqlXml v:
                    Assert.Equal((string)v, result);
                    break;
                case SqlVariant v:
                    switch (v.Value)
                    {
                        case bool b: Assert.Equal(b, result); break;
                        case byte b: Assert.Equal(b, result); break;
                        case short b: Assert.Equal(b, result); break;
                        case int b: Assert.Equal(b, result); break;
                        case long b: Assert.Equal(b, result); break;
                        case float b: Assert.Equal(b, result); break;
                        case double b: Assert.Equal(b, result); break;
                        case DateTime b: Assert.Equal(b, result); break;
                        case Guid b: Assert.Equal(b, result); break;
                        case decimal b: Assert.Equal(b, result); break;
                        case byte[] b: Assert.Equal(b, result); break;
                        case string b: Assert.Equal(b, result); break;
                        case TimeSpan b: Assert.Equal(b, result); break;
                        case DateTimeOffset b: Assert.Equal(b, result); break;
                        default: Assert.False(true);
                            break;
                    }
                    
                    break;
                default:
                    Assert.Equal(value, result);
                    break;
            }

            Assert.Equal(reader.GetReadEndPos(), reader.GetReadPos());
        }

        private static void ObjectWriter(TdsColumnWriter writer, int tdsType, object v, bool writeNull = true)
        {
            switch (tdsType)
            {
                case TdsEnums.SQLBITN when v is bool b: writer.WriteNullableSqlBit(writeNull ? (bool?)null : b, 0); return;
                case TdsEnums.SQLINTN when v is byte b: writer.WriteNullableSqlByte(writeNull ? (byte?)null : b, 0); return;
                case TdsEnums.SQLINTN when v is short b: writer.WriteNullableSqlInt16(writeNull ? (short?)null : b, 0); return;
                case TdsEnums.SQLINTN when v is int b: writer.WriteNullableSqlInt32(writeNull ? (int?)null : b, 0); return;
                case TdsEnums.SQLINTN when v is long b: writer.WriteNullableSqlInt64(writeNull ? (long?)null : b, 0); return;
                case TdsEnums.SQLMONEYN when v is Money4 b: writer.WriteNullableSqlMoney4(writeNull ? (decimal?)null : (decimal?)b, 0); return;
                case TdsEnums.SQLMONEYN when v is Money b: writer.WriteNullableSqlMoney(writeNull ? (decimal?)null : (decimal?)b, 0); return;
                case TdsEnums.SQLFLTN when v is float b: writer.WriteNullableSqlFloat(writeNull ? (float?)null : (float?)b, 0); return;
                case TdsEnums.SQLFLTN when v is double b: writer.WriteNullableSqlDouble(writeNull ? (double?)null : (double?)b, 0); return;
                case TdsEnums.SQLDATE when v is SqlDate b: writer.WriteNullableSqlDate(writeNull ? (DateTime?)null : (DateTime)b, 0); return;
                case TdsEnums.SQLTIME when v is TimeSpan b: writer.WriteNullableSqlTime(writeNull ? (TimeSpan?)null : b, 0); return;
                case TdsEnums.SQLDATETIME2 when v is SqlDateTime2 b: writer.WriteNullableSqlDateTime2(writeNull ? (DateTime?)null : (DateTime)b, 0); return;
                case TdsEnums.SQLDATETIMEOFFSET when v is DateTimeOffset b: writer.WriteNullableSqlDateTimeOffset(writeNull ? (DateTimeOffset?)null : b, 0); return;
                case TdsEnums.SQLDATETIMN when v is SqlDateTime4 b: writer.WriteNullableSqlDateTime(writeNull ? (DateTime?)null : (DateTime)b, 0); return;
                case TdsEnums.SQLDATETIMN when v is DateTime b: writer.WriteNullableSqlDateTime(writeNull ? (DateTime?)null : b, 0); return;
                case TdsEnums.SQLDECIMALN when v is decimal b: writer.WriteNullableSqlDecimal(writeNull ? (decimal?)null : b, 0); return;
                case TdsEnums.SQLUNIQUEID when v is Guid b: writer.WriteNullableSqlUniqueId(writeNull ? (Guid?)null : b, 0); return;
                case TdsEnums.SQLBIGBINARY when v is byte[] b: writer.WriteNullableSqlBinary(writeNull ? (byte[])null : b, 0); return;
                case TdsEnums.SQLBIGVARBINARY when v is byte[] b: writer.WriteNullableSqlBinary(writeNull ? (byte[])null : b, 0); return;
                case TdsEnums.SQLBIGVARCHAR when v is string b: writer.WriteNullableSqlString(writeNull ? (string)null : b, 0); return;
                case TdsEnums.SQLBIGCHAR when v is string b: writer.WriteNullableSqlString(writeNull ? (string)null : b, 0); return;
                case TdsEnums.SQLTEXT when v is string b: writer.WriteNullableSqlString(writeNull ? (string)null : b, 0); return;
                case TdsEnums.SQLNVARCHAR when v is SqlUnicode b: if (writeNull) writer.WriteNullableSqlString(null, 0); else writer.WriteNullableSqlString((string)b, 0); return;
                case TdsEnums.SQLNTEXT when v is SqlUnicode b: if (writeNull) writer.WriteNullableSqlString(null, 0); else writer.WriteNullableSqlString((string)b, 0); return;
                case TdsEnums.SQLNCHAR when v is SqlUnicode b: if (writeNull) writer.WriteNullableSqlString(null, 0); else writer.WriteNullableSqlString((string)b, 0); return;
                case TdsEnums.SQLXMLTYPE when v is SqlXml b: if (writeNull) writer.WriteNullableSqlString(null, 0); else writer.WriteNullableSqlString((string)b, 0); return;
                case TdsEnums.SQLIMAGE when v is SqlImage b: if (writeNull) writer.WriteNullableSqlBinary(null, 0); else writer.WriteNullableSqlBinary((byte[])b, 0); return;
                case TdsEnums.SQLBIT: writer.WriteSqlBit((bool)v, 0); return;
                case TdsEnums.SQLINT1 when v is byte b: writer.WriteSqlByte(b, 0); return;
                case TdsEnums.SQLINT2 when v is short b: writer.WriteSqlInt16(b, 0); return;
                case TdsEnums.SQLINT4 when v is int b: writer.WriteSqlInt32(b, 0); return;
                case TdsEnums.SQLINT8 when v is long b: writer.WriteSqlInt64(b, 0); return;
                case TdsEnums.SQLMONEY4 when v is Money4 b: writer.WriteSqlMoney4((decimal)b, 0); return;
                case TdsEnums.SQLMONEY when v is Money b: writer.WriteSqlMoney((decimal)b, 0); return;
                case TdsEnums.SQLFLT4 when v is float b: writer.WriteSqlFloat(b, 0); return;
                case TdsEnums.SQLFLT8 when v is double b: writer.WriteSqlDouble(b, 0); return;
                case TdsEnums.SQLDATETIM4 when v is SqlDateTime4 b: writer.WriteSqlDateTime4((DateTime)b, 0); return;
                case TdsEnums.SQLDATETIME when v is DateTime b: writer.WriteSqlDateTime(b, 0); return;
                case TdsEnums.SQLVARIANT when v is SqlVariant b: if (writeNull) writer.WriteSqlVariant(null, 0); else writer.WriteSqlVariant(b.Value, 0); return;
            }

            throw new NotImplementedException();
        }

        private static object ObjectReader(TdsColumnReader reader, object o)
        {
            switch (o)
            {
                case bool _: return reader.ReadSqlBit(0); ;
                case byte _: return reader.ReadSqlByte(0); ;
                case short _: return reader.ReadSqlInt16(0); ;
                case int _: return reader.ReadSqlInt32(0); ;
                case long _: return reader.ReadSqlInt64(0); ;
                case Money4 _: return reader.ReadSqlMoney(0); ;
                case Money _: return reader.ReadSqlMoney(0); ;
                case float _: return reader.ReadSqlFloat(0); ;
                case double _: return reader.ReadSqlDouble(0); ;
                case SqlDate _: return reader.ReadSqlDate(0); ;
                case TimeSpan _: return reader.ReadSqlTime(0); ;
                case SqlDateTime2 _: return reader.ReadSqlDateTime2(0); ;
                case DateTimeOffset _: return reader.ReadSqlDateTimeOffset(0); ;
                case SqlDateTime4 _: return reader.ReadSqlDateTime(0); ;
                case DateTime _: return reader.ReadSqlDateTime(0); ;
                case decimal _: return reader.ReadDecimal(0); ;
                case Guid _: return reader.ReadSqlGuid(0); ;
                case byte[] _: return reader.ReadBinary(0); ;
                case SqlImage _: return reader.ReadBinary(0); ;
                case string _: return reader.ReadString(0); ;
                case SqlUnicode _: return reader.ReadUnicodeString(0); ;
                case SqlXml _: return reader.ReadUnicodeString(0); ;
                case SqlVariant _: return reader.ReadSqlVariant(0); ;
                //case SqlUnicode _: return reader.Readsql(0); ;
            }

            throw new NotImplementedException();
        }

        private void SetupColMetaData(TdsPackageReader reader, TdsPackageWriter writer, int tdsType, byte precision, byte scale, bool isPlp)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var r = reader.CurrentResultset.ColumnsMetadata = new ColumnsMetadata(1);
            var w = writer.ColumnsMetadata = new MetadataBulkCopy[1];
            var collation = new SqlCollations() { Info = 0x00d00409, SortId = 0x34 };
            var encoding = Encoding.GetEncoding(collation.GetCodePage());
            w[0] = new MetadataBulkCopy();
            r[0].TdsType = (byte)tdsType;
            w[0].TdsType = (byte)tdsType;
            r[0].Scale = scale;
            w[0].Scale = scale;
            r[0].IsPlp = isPlp;
            w[0].IsPlp = isPlp;
            r[0].IsPlp = isPlp;

            r[0].Encoding = encoding;
            w[0].Encoding = encoding;
            w[0].Collation = collation;

            w[0].Precision = precision;

            w[0].MetaType = TdsMetaType.TdsTypes[tdsType];
            r[0].MetaType = TdsMetaType.TdsTypes[tdsType];

            r[0].IsTextOrImage = r[0].MetaType.IsTextOrImage;
            w[0].IsTextOrImage = r[0].MetaType.IsTextOrImage;
        }
    }

    public struct Money4
    {
        private decimal? Value { get; }

        public Money4(decimal? v)
        {
            Value = v;
        }

        public static explicit operator Money4(decimal? value)
        {
            return new Money4(value);
        }

        public static explicit operator Money4(decimal value)
        {
            return new Money4(value);
        }

        public static explicit operator decimal? (Money4 me)
        {
            return me.Value;
        }

        public static explicit operator decimal(Money4 me)
        {
            return (decimal)me.Value;
        }
    }

    public struct Money
    {
        private decimal? Value { get; }

        public Money(decimal? v)
        {
            Value = v;
        }

        public static explicit operator Money(decimal? value)
        {
            return new Money(value);
        }

        public static explicit operator Money(decimal value)
        {
            return new Money(value);
        }

        public static explicit operator decimal? (Money me)
        {
            return me.Value;
        }

        public static explicit operator decimal(Money me)
        {
            return (decimal)me.Value;
        }
    }

    public struct SqlDate
    {
        private DateTime? Value { get; }
        public SqlDate(DateTime? v) { Value = v; }
        public static explicit operator SqlDate(DateTime? value) { return new SqlDate(value); }
        public static explicit operator SqlDate(DateTime value) { return new SqlDate(value); }
        public static explicit operator DateTime? (SqlDate me) { return me.Value; }
        public static explicit operator DateTime(SqlDate me) { return (DateTime)me.Value; }
    }
    public struct SqlDateTime2
    {
        private DateTime? Value { get; }
        public SqlDateTime2(DateTime? v) { Value = v; }
        public static explicit operator SqlDateTime2(DateTime? value) { return new SqlDateTime2(value); }
        public static explicit operator SqlDateTime2(DateTime value) { return new SqlDateTime2(value); }
        public static explicit operator DateTime? (SqlDateTime2 me) { return me.Value; }
        public static explicit operator DateTime(SqlDateTime2 me) { return (DateTime)me.Value; }
    }

    public struct SqlDateTime4
    {
        private DateTime? Value { get; }
        public SqlDateTime4(DateTime? v) { Value = v; }
        public static explicit operator SqlDateTime4(DateTime? value) { return new SqlDateTime4(value); }
        public static explicit operator SqlDateTime4(DateTime value) { return new SqlDateTime4(value); }
        public static explicit operator DateTime? (SqlDateTime4 me) { return me.Value; }
        public static explicit operator DateTime(SqlDateTime4 me) { return (DateTime)me.Value; }
    }
    public struct SqlImage
    {
        private Byte[] Value { get; }
        public SqlImage(Byte[] v) { Value = v; }
        public static explicit operator SqlImage(Byte[] value) { return new SqlImage(value); }
        public static explicit operator Byte[] (SqlImage me) { return (Byte[])me.Value; }
    }
    public struct SqlUnicode
    {
        private string Value { get; }
        public SqlUnicode(string v) { Value = v; }
        public static explicit operator SqlUnicode(string value) { return new SqlUnicode(value); }
        public static explicit operator string(SqlUnicode me) { return (string)me.Value; }
    }
    public struct SqlXml
    {
        private string Value { get; }
        public SqlXml(string v) { Value = v; }
        public static explicit operator SqlXml(string value) { return new SqlXml(value); }
        public static explicit operator string(SqlXml me) { return (string)me.Value; }
    }
    public struct SqlVariant
    {
        public object Value { get; }
        public SqlVariant(object v) { Value = v; }
        //public static explicit operator SqlVariant(object value) { return new SqlVariant(value); }
        //public static explicit operator object(SqlVariant me) { return (object)me.Value; }
    }
}