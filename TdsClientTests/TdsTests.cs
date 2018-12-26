using System;
using System.Threading.Tasks;
using Medella.TdsClient.Contants;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.TdsStream.SniNp;
using Medella.TdsClient.TDS;
using Medella.TdsClient.TDS.Controller;
using Medella.TdsClient.TDS.Messages.Client;
using Medella.TdsClient.TDS.Messages.Server;
using Medella.TdsClient.TDS.Package.Reader;
using Medella.TdsClient.TDS.Package.Writer;
using Medella.TdsClient.TDS.Processes;
using Xunit;

namespace TdsClientTests
{
    public class TdsTests
    {
        private const string ConnectionString = @"Server=tcp:.,1433;Database=tmp;Trusted_Connection=True;";
        private const string ConnectionString2 = @"Server=.;Database=tmp;Trusted_Connection=True;";

        [Fact]
        public async Task can_login()
        {
            var tds = TdsConnectionPools.GetConnectionPool(ConnectionString);
            await tds.ExecuteNonQueryAsync("print 1");
            tds = TdsConnectionPools.GetConnectionPool(ConnectionString);
            await tds.ExecuteNonQueryAsync("print 1");
        }

        [Fact]
        public async Task can_login_toSql()
        {
            var tds = TdsConnectionPools.GetConnectionPool(ConnectionString2);
            await tds.ExecuteNonQueryAsync("print 1");
        }

        [Fact]
        public async Task can_read_a_large_column()
        {
            var tds = TdsConnectionPools.GetConnectionPool(ConnectionString);
            var x = await tds.ExecuteQueryAsync<NotNullTypes>(TestStatements.NotNullTable);
        }

        [Fact]
        public async Task can_read_not_null_fields()
        {
            var tds = TdsConnectionPools.GetConnectionPool(ConnectionString);
            var x = await tds.ExecuteQueryAsync<NotNullTypes>(TestStatements.NotNullTable);
        }

        [Fact]
        public async Task can_read_timestamp()
        {
            var tds = TdsConnectionPools.GetConnectionPool(ConnectionString);
            var x = await tds.ExecuteQueryAsync<TestIntType>(@"DECLARE @zz timestamp; Select zz=@zz");
        }

        [Fact]
        public async Task can_Serailize_all_nulls()
        {
            var tds = TdsConnectionPools.GetConnectionPool(ConnectionString);
            var x = await tds.ExecuteQueryAsync<TestNullType>(TestStatements.SelectAllNullTypes);
            var v = x[0];
            Assert.Null(v.a);
            Assert.Null(v.b);
            Assert.Null(v.c);
            Assert.Null(v.d);
            Assert.Null(v.e);
            Assert.Null(v.f);
            Assert.Null(v.g);
            Assert.Null(v.h);
            Assert.Null(v.i);
            Assert.Null(v.j);
            Assert.Null(v.k);
            Assert.Null(v.l);
            Assert.Null(v.m);
            Assert.Null(v.n);
            Assert.Null(v.o);
            Assert.Null(v.p);
            Assert.Null(v.q);
            Assert.Null(v.r);
            Assert.Null(v.s);
            Assert.Null(v.t);
            Assert.Null(v.u);
            Assert.Null(v.v);
            Assert.Null(v.w);
            Assert.Null(v.x);
            Assert.Null(v.y);
            Assert.Null(v.z);
            Assert.Null(v.zz);
        }

        [Fact]
        public async Task can_Serailize_normal_sqltype_to_object()
        {
            var tds = TdsConnectionPools.GetConnectionPool(ConnectionString);
            var x = await tds.ExecuteQueryAsync<TestNullType>(TestStatements.SelectAllNotNullTypes);
            var v = x[0].a == null ? x[1] : x[0];
            Assert.Equal(new DateTime(2018, 12, 31), v.a);
            Assert.Equal(new TimeSpan(10, 11, 12), v.b);
            Assert.Equal(new DateTime(2018, 12, 31, 10, 11, 12), v.c);
            Assert.Equal(new DateTimeOffset(2018, 12, 31, 10, 11, 12, new TimeSpan(5, 0, 0)), v.d);
            Assert.True(v.e);
            Assert.Equal((byte?) 1, v.f);
            Assert.Equal((short?) 1, v.g);
            Assert.Equal(1, v.h);
            Assert.Equal(1, v.i);
            Assert.Equal(1, v.j);
            Assert.Equal(1, v.k);
            Assert.Equal(1, v.l);
            Assert.Equal(1, v.m);
            Assert.Equal(new DateTime(2018, 12, 31), v.n);
            Assert.Equal(new DateTime(2018, 12, 31), v.o);
            Assert.Equal(new Guid("9e383328-69d7-4e73-8126-e25a1be94ae9"), v.p);
            Assert.Equal(new byte[] {1}, v.q);
            Assert.Equal(new byte[] {49, 50, 51, 52, 53, 54, 55, 56, 57}, v.r);
            Assert.Equal(new[] {'1'}, v.s);
            Assert.Equal("123456789", v.t);
            Assert.Equal(new[] {'1'}, v.u);
            Assert.StartsWith("123456789", v.v);
            Assert.Equal(1, v.x);
            Assert.Equal(1_000_000_000_000_000_000, v.y);
            Assert.Equal(9_999_999_999_999_999_583_119_736_832M, v.z);
            Assert.Equal(new byte[] {0, 0, 0, 0, 0, 0, 0, 1}, v.zz);
        }

        [Fact]
        public async Task can_Serailize_normal_varianttype_to_object()
        {
            var tds = TdsConnectionPools.GetConnectionPool(ConnectionString);
            var x = await tds.ExecuteQueryAsync<TestNullType>(TestStatements.SelectAllVariantTypes);
            var v = x[0].a == null ? x[1] : x[0];
            Assert.Equal(new DateTime(2018, 12, 31), v.a);
            Assert.Equal(new TimeSpan(10, 11, 12), v.b);
            Assert.Equal(new DateTime(2018, 12, 31, 10, 11, 12), v.c);
            Assert.Equal(new DateTimeOffset(2018, 12, 31, 10, 11, 12, new TimeSpan(5, 0, 0)), v.d);
            Assert.True(v.e);
            Assert.Equal((byte?) 1, v.f);
            Assert.Equal((short?) 1, v.g);
            Assert.Equal(1, v.h);
            Assert.Equal(1, v.i);
            Assert.Equal(1, v.j);
            Assert.Equal(1, v.k);
            Assert.Equal(1, v.l);
            Assert.Equal(1, v.m);
            Assert.Equal(new DateTime(2018, 12, 31), v.n);
            Assert.Equal(new DateTime(2018, 12, 31), v.o);
            Assert.Equal(new Guid("9e383328-69d7-4e73-8126-e25a1be94ae9"), v.p);
            Assert.Equal(new byte[] {1}, v.q);
            Assert.Equal(new[] {'1'}, v.s);
            Assert.Equal(new[] {'1'}, v.u);
            Assert.Equal(1, v.x);
            Assert.Equal(1_000_000_000_000_000_000, v.y);
            Assert.Equal(9_999_999_999_999_999_583_119_736_832M, v.z);
        }

      


        [Fact]
        public void get_connection_properties()
        {
            Assert.True(ServerConnectionOptions.IsNamedPipe(@"np:\\.\pipe\LOCALDB#678E2031\tsql\query".Split(@"\")));
            //Act
            var p = new ServerConnectionOptions(@"(localdb)\mssqllocaldb", false);
            //
            Assert.StartsWith(@"localdb#", p.PipeName);
            Assert.EndsWith(@"\tsql\query", p.PipeName);
            Assert.Equal(@".", p.PipeServerName);

            //Act
            p = new ServerConnectionOptions(@"tcp:localhost,1444", false);
            //
            Assert.Equal(@"localhost", p.IpServerName);
            Assert.Null(p.InstanceName);
            Assert.Equal(1444, p.IpPort);
            Assert.False(p.IsSsrpRequired);
            //Act
            p = new ServerConnectionOptions(@"tcp:localhost\instance", false);
            //Assert
            Assert.Equal(@"localhost", p.IpServerName);
            Assert.Equal("instance", p.InstanceName);
            Assert.Equal(-1, p.IpPort);
            Assert.True(p.IsSsrpRequired);
        }

        [Fact]
        public void get_npSniHandle()
        {
            const bool marsOn = false;
            var p = new ServerConnectionOptions(@"(localdb)\mssqllocaldb", false);
            var handle = new TdsStreamNamedpipes(p.PipeServerName, p.PipeName, 15);
            var writer = new TdsPackageWriter(handle);
            var reader = new TdsPackageReader(handle);

            writer.SendPreLoginHandshake("", marsOn);
            reader.CheckBuffer(8);
            var result = ParserPreLogin.ParsePreLoginHandshake(reader.ReadBuffer, TdsEnums.HEADER_LEN, EncryptionOptions.OFF);
            Assert.Equal(EncryptionOptions.OFF, result);
        }

        [Fact]
        public async Task ReadLargeColumn()
        {
            var cnn = TdsConnectionPools.GetConnectionPool(ConnectionString);
            var result = await cnn.ExecuteQueryAsync<TestLongStringType>(@"
                DECLARE @v nvarchar(max)=CAST( replicate('1',32768) AS nvarchar(max) )
                SELECT s=@v");
            Assert.Equal(8000, result[0].s.Length);
        }

        [Fact]
        public async Task ReadLargeColumn2()
        {
            var tds = TdsConnectionPools.GetConnectionPool(@"Server=(localdb)\mssqllocaldb;Database=tmp;Trusted_Connection=True;");
            {
	            tds.ExecuteNonQuery("if OBJECT_ID('TestVarCharMax') is not null DROP TABLE TestVarCharMax CREATE TABLE TestVarCharMax (test varchar(max))");
	            tds.ExecuteNonQuery("Insert TestVarCharMax (test) values (replicate(CONVERT(VARCHAR(MAX),'x'), 16000))");
				var result = await tds.ExecuteQueryAsync<TestLongStringType>(@"SELECT s=test FROM TestVarCharMax");
                Assert.Equal(16000, result[0].s.Length);
            }
        }

        [Fact]
        public async Task ReadWmptyXml()
        {
            var tds = TdsConnectionPools.GetConnectionPool(@"Server=(localdb)\mssqllocaldb;Database=tmp;Trusted_Connection=True;");
            {
                var result = await tds.ExecuteQueryAsync<TestLongStringType>(@"SELECT s=cast('' as xml)");
                ;
                Assert.Equal(0, result[0].s.Length);
            }
        }

        [Fact]
        public async Task ReadXml()
        {
            var tds = TdsConnectionPools.GetConnectionPool(@"Server=(localdb)\mssqllocaldb;Database=tmp;Trusted_Connection=True;");
            {
                var result = await tds.ExecuteQueryAsync<TestLongStringType>(@"
DECLARE @v xml=CAST( '<Data><DepartmentID>x</DepartmentID></Data>' AS xml )
SELECT s=@v");
                ;
                Assert.Equal(43, result[0].s.Length);
            }
        }
    }

    public class ConnectionPoolTests
    {
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=tmp;Trusted_Connection=True;";
        [Fact]
        public void Connection_returned_cleaned_to_the_pool()
        {
            var cnn = TdsConnectionPools.GetConnectionPool(ConnectionString);
            var cnninternal = cnn.GetConnection();

            cnninternal.ExecuteNonQuery("print 1");
            Assert.Equal(3, cnninternal.SqlMessages.Count);
            cnn.Return(cnninternal);

            cnninternal = cnn.GetConnection();
            cnninternal.ExecuteNonQuery("print 1");
            Assert.Equal(3, cnninternal.SqlMessages.Count);
            cnn.Return(cnninternal);

            cnninternal = cnn.GetConnection();
            Assert.Equal(2, cnninternal.SqlMessages.Count);
            cnn.Return(cnninternal);
        }

        [Fact]
        public void Get_50_connections_return_to_the_pool_and_use_one()
        {
            var x = new TdsConnection[50];
            var tds = TdsConnectionPools.GetConnectionPool(ConnectionString);
            for (int j = 0; j < 50; j++)
                x[j] = tds.GetConnection();
            //OrmTester.EnsureDbSetup(_connectionString);
            for (int j = 0; j < 50; j++)
                tds.Return(x[j]);
        }
    }
    public class UdtTypes
    {
        public byte[] Utf8String { get; set; }
    }
}