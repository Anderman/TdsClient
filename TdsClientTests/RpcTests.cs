using System;
using System.Text;
using System.Threading.Tasks;
using Medella.TdsClient.TDS;
using Medella.TdsClient.TDS.Controller;
using Medella.TdsClient.TDS.Messages.Client;
using Medella.TdsClient.TDS.Messages.Server;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Processes;
using Medella.TdsClient.TDS.Reader.StringHelpers;
using Xunit;

namespace TdsClientTests
{
    public class RpcTests
    {
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=tmp;Trusted_Connection=True;";

        public static byte[] ExpectedString =
        {
            0x03, 0x01, 0x00, 0xD4, 0x00, 0x00, 0x01, 0x00, //header
            0x16, 0x00, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, //WriteMarsHeader
            0xFF, 0xFF, 0x0A, 0x00, //RPC
            0x02, 0x00, //TdsEnums.RPC_PARAM_DEFAULT

            0x00, //Bytestring
            0x00, //Status
            0xE7, //TdsType
            0x48, 0x00, //len
            0x09, 0x04, 0xD0, 0x00, 0x34, //info, sort
            0x48, 0x00, //Len
            0x53, 0x00, 0x45, 0x00, 0x4C, 0x00, 0x45, 0x00, 0x43, 0x00, 0x54, 0x00, 0x20, 0x00, 0x63, 0x00, 0x61, 0x00, 0x73, 0x00, 0x74, 0x00, 0x28, 0x00, 0x31, 0x00, 0x20, 0x00, 0x61, 0x00, 0x73, 0x00, 0x20, 0x00, 0x62, 0x00, 0x69, 0x00, 0x74, 0x00, 0x29, 0x00, 0x2C, 0x00, 0x20, 0x00, 0x67, 0x00, 0x65, 0x00, 0x74, 0x00, 0x44, 0x00, 0x61, 0x00, 0x74, 0x00, 0x65, 0x00, 0x28, 0x00, 0x29, 0x00,
            0x2C, 0x00, 0x40, 0x00, 0x70, 0x00, 0x30, 0x00,

            0x00, //Bytestring
            0x00, //Status
            0xE7, //TdsType
            0x1e, 0x00, //len
            0x09, 0x04, 0xD0, 0x00, 0x34, //info, sort
            0x1e, 0x00, //len
            0x40, 0x00, 0x70, 0x00, 0x30, 0x00, 0x20, 0x00, 0x6E, 0x00, 0x76, 0x00, 0x61, 0x00, 0x72, 0x00, 0x63, 0x00, 0x68, 0x00, 0x61, 0x00, 0x72, 0x00, 0x28, 0x00, 0x37, 0x00, 0x29, 0x00,

            0x03, 0x40, 0x00, 0x70, 0x00, 0x30, 0x00, //bytestring = @p0
            0x00, //Status
            0xE7, //TdsType
            0x0E, 0x00, //len
            0x09, 0x04, 0xD0, 0x00, 0x34, //info,sort
            0x0E, 0x00, //len
            0x74, 0x00, 0x65, 0x00, 0x73, 0x00, 0x74, 0x00, 0x31, 0x00, 0x32, 0x00, 0x33, 0x00
        };

        public static byte[] ExpectedDecimal =
        {
            0x03, 0x01, 0x00, 0x90, 0x00, 0x00, 0x01, 0x00,
            0x16, 0x00, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0xFF, 0xFF, 0x0A, 0x00,
            0x02, 0x00,
            0x00, //Bytestring
            0x00, //Status
            0xE7, //TdsType
            0x14, 0x00,
            0x09, 0x04, 0xD0, 0x00, 0x34,
            0x14, 0x00,
            0x53, 0x00, 0x45, 0x00, 0x4C, 0x00, 0x45, 0x00, 0x43, 0x00, 0x54, 0x00, 0x20, 0x00, 0x40, 0x00, 0x70, 0x00, 0x30, 0x00,
            0x00, //Bytestring
            0x00, //Status
            0xE7, //TdsType
            0x22, 0x00,
            0x09, 0x04, 0xD0, 0x00, 0x34,
            0x22,
            0x00, 0x40, 0x00, 0x70, 0x00, 0x30, 0x00, 0x20, 0x00, 0x64, 0x00, 0x65, 0x00, 0x63, 0x00, 0x69, 0x00, 0x6D, 0x00, 0x61, 0x00, 0x6C, 0x00, 0x28, 0x00, 0x32, 0x00, 0x38, 0x00, 0x2C, 0x00, 0x33, 0x00, 0x29, 0x00,
            0x03, 0x40, 0x00, 0x70, 0x00, 0x30, 0x00, //bytestring = @p0
            0x00,
            0x6C, //TdsType
            0x11, //len
            0x1C, //Precision
            0x03, //scale
            0x11, //len
            0x01, //positive
            0xF3, 0xE0, 0x01, 0x00, //intLo
            0x00, 0x00, 0x00, 0x00, //intMid
            0x00, 0x00, 0x00, 0x00, //intHi
            0x00, 0x00, 0x00, 0x00 //int3
        };

        public static byte[] ExpectedBool =
        {
            0x03, 0x01, 0x00, 0x90, 0x00, 0x00, 0x01, 0x00,
            0x16, 0x00, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0xFF, 0xFF, 0x0A, 0x00,
            0x02, 0x00,
            0x00, //Bytestring
            0x00, //Status
            0xE7, //TdsType
            0x14, 0x00,
            0x09, 0x04, 0xD0, 0x00, 0x34,
            0x14, 0x00,
            0x53, 0x00, 0x45, 0x00, 0x4C, 0x00, 0x45, 0x00, 0x43, 0x00, 0x54, 0x00, 0x20, 0x00, 0x40, 0x00, 0x70, 0x00, 0x30, 0x00,
            0x00, //Bytestring
            0x00, //Status
            0xE7, //TdsType
            0x0E, 0x00,
            0x09, 0x04, 0xD0, 0x00, 0x34,
            0x0E, 0x00,
            0x40, 0x00, 0x70, 0x00, 0x30, 0x00, 0x20, 0x00, 0x62, 0x00, 0x69, 0x00, 0x74, 0x00,
            0x03, 0x40, 0x00, 0x70, 0x00, 0x30, 0x00, //bytestring = @p0
            0x00,
            0x68, //TdsType
            0x01, //len
            0x01, //len
            0x01 //value
        };

        private byte[] ExpectedBigVarBinary = {0xA5, 0xFF, 0xFF, /*long*/0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, /*chunk*/0x00, 0x00, 0x01, 0x00};
        private byte[] ExpectedBigBinary = {0xAD, 0x01, 0x00, 0x01, 0x00, 0xFF};
        private byte[] ExpectedByte = {0x26, 0x01, 0x01, 0x01};
        private byte[] ExpectedInt16 = {0x26, 0x02, 0x02, 0x01, 0x00};
        private byte[] ExpectedInt32 = {0x26, 0x04, 0x04, 0x01, 0x00, 0x00, 0x00};
        private byte[] ExpectedInt64 = {0x26, 0x08, 0x08, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
        private readonly byte[] ExpectedDatetime = {0x6F, 0x08, 0x08, 0x5C, 0xA8, 0x00, 0x00, 0x5C, 0x8F, 0x32, 0x00};
        private readonly SqlCollations _defaultCollation = new SqlCollations {Info = 0x00_d0_04_09, SortId = 0x34};


        public class TestId
        {
            public string StringValue { get; set; }
            public decimal Decimal { get; set; }
            public byte cByte { get; set; }
            public short cShort { get; set; }
            public int cInt { get; set; }
            public long clong { get; set; }
            public double cDouble { get; set; }
            public DateTime cDateTime { get; set; }
        }

        private static string GetBytesString(byte[] buffer, int length, int offset = 8)
        {
            var sb = new StringBuilder();
            sb.Append("data: ");
            for (var i = offset; i < length; i++)
                sb.Append($"{buffer[i],2:X2} ");
            sb.Append($"{length,4:##0} ");
            return sb.ToString();
        }


        [Fact]
        public void can_calculate_scale()
        {
            var scale = new TdsParameter("", 123.123m).Scale;
            Assert.Equal(3, scale);
        }

        [Fact]
        public void Can_createParameters()
        {
            var id = "123";
            var pars = WriterExecuteRpc.CreateParameters($"select * where id={id}");
            Assert.Equal("select * where id=@p0", pars[0].Value);
            Assert.Equal("@p0 nvarchar(3)", pars[1].Value);
            Assert.Equal("@p0", pars[2].Name);
            Assert.Equal("123", pars[2].Value);
        }

        [Fact]
        public void Can_execute_query_with_byte_parameter()
        {
            var tds = TdsClient.GetConnection(ConnectionString);
            var id = (byte) 0xff;
            tds.ExecuteParameterQueryASync<TestId>($"select cByte={id}");
        }

        [Fact]
        public async Task Can_execute_query_with_datetime_parameter()
        {
            var tds = TdsClient.GetConnection(ConnectionString);
            var id = new DateTime(2018, 1, 2, 3, 4, 5);
            var r =(await tds.ExecuteParameterQueryASync<TestId>($"select cDateTime={id}")).ToArray();
            Assert.Equal(new DateTime(2018, 1, 2, 3, 4, 5), r[0].cDateTime);
        }

        [Fact]
        public void Can_execute_query_with_decimal_parameter()
        {
            var tds = TdsClient.GetConnection(ConnectionString);
            var id = 123.4M;
            tds.ExecuteParameterQueryASync<TestId>($"select Decimal={id}");
        }

        [Fact]
        public async Task Can_execute_query_with_double_parameterAsync()
        {
            var tds = TdsClient.GetConnection(ConnectionString);
            var id = double.MaxValue;
            var r = (await tds.ExecuteParameterQueryASync<TestId>($"select cDouble={id}")).ToArray();
            Assert.Equal(double.MaxValue, r[0].cDouble);
        }

        [Fact]
        public async Task Can_execute_query_with_float_parameter()
        {
            var tds = TdsClient.GetConnection(ConnectionString);
            var id = float.MaxValue;
            var r = (await tds.ExecuteParameterQueryASync<TestId>($"select cDouble={id}")).ToArray();
            Assert.Equal(float.MaxValue, r[0].cDouble);
        }

        [Fact]
        public async Task Can_execute_query_with_int_parameter()
        {
            var tds = TdsClient.GetConnection(ConnectionString);
            var id = int.MaxValue;
            var r = await tds.ExecuteParameterQueryASync<TestId>($"select cInt={id}");
            Assert.Equal(int.MaxValue, r[0].cInt);
        }

        [Fact]
        public async Task Can_execute_query_with_long_parameter()
        {
            var tds = TdsClient.GetConnection(ConnectionString);
            var id = long.MaxValue;
            var r = (await tds.ExecuteParameterQueryASync<TestId>($"select clong={id}")).ToArray();
            Assert.Equal(long.MaxValue, r[0].clong);
        }

        [Fact]
        public async Task Can_execute_query_with_short_parameter()
        {
            var tds = TdsClient.GetConnection(ConnectionString);
            var id = short.MaxValue;
            var r = await tds.ExecuteParameterQueryASync<TestId>($"select cShort={id}");
            Assert.Equal(short.MaxValue, r[0].cShort);
        }

        [Fact]
        public async Task Can_execute_query_with_string_parameter()
        {
            var tds = TdsClient.GetConnection(ConnectionString);
            var id = "test123";
            await tds.ExecuteParameterQueryASync<TestId>($"select StringValue={id}");
        }

        [Fact]
        public async Task Can_execute_query_with_two_parameter()
        {
            var tds = TdsClient.GetConnection(ConnectionString);
            var id = new DateTime(2018, 1, 2, 3, 4, 5);
            var clong = long.MaxValue;
            var r = (await tds.ExecuteParameterQueryASync<TestId>($"select cDateTime={id}, clong={clong}")).ToArray();
            Assert.Equal(new DateTime(2018, 1, 2, 3, 4, 5), r[0].cDateTime);
        }

        [Fact]
        public void can_execute_rpc_with_bool_parameter()
        {
            var stream = new TestStream();
            var writer = new TdsPackageWriter(stream);
            var input = true;

            FormattableString sql = $"SELECT {input}";
            writer.SendRpc(_defaultCollation, sql,0);
            Assert.Equal(GetBytesString(ExpectedBool, ExpectedBool.Length), GetBytesString(writer.WriteBuffer, ExpectedBool.Length));
        }


        [Fact]
        public void can_execute_rpc_with_decimal_parameter()
        {
            var stream = new TestStream();
            var writer = new TdsPackageWriter(stream);
            var input = 123.123M;

            FormattableString sql = $"SELECT {input}";
            writer.SendRpc(_defaultCollation, sql,0);
            Assert.Equal(GetBytesString(ExpectedDecimal, ExpectedDecimal.Length), GetBytesString(writer.WriteBuffer, ExpectedDecimal.Length));
        }

        [Fact]
        public void Can_execute_rpc_with_string_parameter()
        {
            var stream = new TestStream();
            var writer = new TdsPackageWriter(stream);
            var input = "test123";
            FormattableString sql = $"SELECT cast(1 as bit), getDate(),{input}";

            writer.SendRpc(_defaultCollation, sql,0);
            Assert.Equal(GetBytesString(ExpectedString, ExpectedString.Length), GetBytesString(writer.WriteBuffer, ExpectedString.Length));
        }

        [Fact]
        public void can_write_datetime_format()
        {
            var stream = new TestStream();
            var writer = new TdsPackageWriter(stream);

            var p = new TdsParameter("", new DateTime(2018, 1, 2, 3, 4, 5));
            writer.WriteTdsTypeInfo(p.MetaData, 8, false);
            writer.WriteDateTime((DateTime) p.Value);
            var sqldt = TdsPackageWriter.ConverDateTime((DateTime) p.Value);

            var dt = ParserColumnDateExtentions.ConvertSqlDate(sqldt.dayPart, sqldt.timepart);
            dt = ParserColumnDateExtentions.ConvertSqlDate(0xa85c, 0x328f5c);
            Assert.Equal(new DateTime(2018, 1, 2, 3, 4, 5), dt);
            Assert.Equal(0x328f5c, sqldt.timepart);
            Assert.Equal(GetBytesString(ExpectedDatetime, ExpectedDatetime.Length, 0), GetBytesString(writer.WriteBuffer, ExpectedDatetime.Length, 0));
        }
    }
}