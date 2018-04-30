using System;
using Medella.TdsClient.Contants;
using Medella.TdsClient.Exceptions;
using Medella.TdsClient.SNI.SniNp;
using Medella.TdsClient.TDS;
using Medella.TdsClient.TDS.Messages.Client;
using Medella.TdsClient.TDS.Messages.Server;
using Medella.TdsClient.TDS.Package;
using Xunit;
using ServerConnectionOptions = XUnitTestProject1.ServerConnectionOptions;

namespace TdsClientTests
{
    public class TdsTests
    {
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=test;Trusted_Connection=True;";
        private const string ConnectionString2 = @"context connection=true";

        public class TestIntType
        {
            public DateTime a { get; set; }
            public TimeSpan b { get; set; }
            public DateTime c { get; set; }
            public DateTimeOffset d { get; set; }
            public bool e { get; set; }
            public byte f { get; set; }
            public short g { get; set; }
            public int h { get; set; }
            public long i { get; set; }
            public float j { get; set; }
            public double k { get; set; }
            public decimal l { get; set; }
            public decimal m { get; set; }
            public DateTime n { get; set; }
            public DateTime o { get; set; }
            public Guid p { get; set; }
            public byte[] q { get; set; }
            public byte[] r { get; set; }
            public string s { get; set; }
            public string t { get; set; }
            public string u { get; set; }
            public string v { get; set; }
            public string w { get; set; }
            public decimal x { get; set; }
            public decimal y { get; set; }
            public decimal z { get; set; }
            public byte[] zz { get; set; }
        }

        public class TestNullType
        {
            public DateTime? a { get; set; }
            public TimeSpan? b { get; set; }
            public DateTime? c { get; set; }
            public DateTimeOffset? d { get; set; }
            public bool? e { get; set; }
            public byte? f { get; set; }
            public short? g { get; set; }
            public int? h { get; set; }
            public long? i { get; set; }
            public float? j { get; set; }
            public double? k { get; set; }
            public decimal? l { get; set; }
            public decimal? m { get; set; }
            public DateTime? n { get; set; }
            public DateTime? o { get; set; }
            public Guid? p { get; set; }
            public byte[] q { get; set; }
            public byte[] r { get; set; }
            public string s { get; set; }
            public string t { get; set; }
            public string u { get; set; }
            public string v { get; set; }
            public string w { get; set; }
            public decimal? x { get; set; }
            public decimal? y { get; set; }
            public decimal? z { get; set; }
            public byte[] zz { get; set; }
        }

        [Fact]
        public void can_login()
        {
            var tds = new Tds(ConnectionString);
            tds.Connect();
            tds.Login();
            tds.ExecuteNonQuery("print 1");
        }

        [Fact]
        public void can_read_not_fields()
        {
            var tds = new Tds(ConnectionString);
            tds.Connect();
            tds.Login();
            var x = tds.ExecuteQuery<NotNullTypes>(TestStatements.NotNullTable);
        }

        [Fact]
        public void can_read_timestamp()
        {
            var tds = new Tds(ConnectionString);
            tds.Connect();
            tds.Login();
            var x = tds.ExecuteQuery<TestIntType>(@"DECLARE @zz timestamp; Select zz=@zz");
        }

        [Fact]
        public void can_Serailize_all_nulls()
        {
            var tds = new Tds(ConnectionString);
            tds.Connect();
            tds.Login();
            var x = tds.ExecuteQuery<TestNullType>(TestStatements.SelectAllNullTypes);
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
        public void can_Serailize_normal_sqltype_to_object()
        {
            var tds = new Tds(ConnectionString);
            tds.Connect();
            tds.Login();
            var x = tds.ExecuteQuery<TestNullType>(TestStatements.SelectAllNotNullTypes);
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
            Assert.Equal(1, v.x);
            Assert.Equal(1_000_000_000_000_000_000, v.y);
            Assert.Equal(9_999_999_999_999_999_583_119_736_832M, v.z);
            Assert.Equal(new byte[] {0, 0, 0, 0, 0, 0, 0, 1}, v.zz);
        }

        [Fact]
        public void can_Serailize_normal_varianttype_to_object()
        {
            var tds = new Tds(ConnectionString);
            tds.Connect();
            tds.Login();
            var x = tds.ExecuteQuery<TestNullType>(TestStatements.SelectAllVariantTypes);
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
            var handle = new SniNpHandle(p.PipeServerName, p.PipeName, 15);
            var writer = new TdsPackageWriter(handle);
            var reader = new TdsPackageReader(handle);

            writer.SendPreLoginHandshake(p.InstanceNameBytes, marsOn);
            var status = reader.ReadPackage();
            Assert.Equal(TdsEnums.ST_EOM, status);
            var result = ParserPreLogin.ParsePreLoginHandshake(reader.ReadBuffer, TdsEnums.HEADER_LEN, EncryptionOptions.OFF);
            Assert.Equal(13, result.Version);
            Assert.False(result.IsMarsCapable);
            Assert.True(result.IsYukonOrLater);
            Assert.Equal(EncryptionOptions.OFF, result.EncryptionOption);
        }
    }

    public class NotNullTypes
    {
        public int nId { get; set; }
        public DateTime ndate { get; set; }
        public TimeSpan ntime { get; set; }
        public DateTime ndatetime2 { get; set; }
        public DateTimeOffset ndatetimeoffset { get; set; }
        public bool nbit { get; set; }
        public byte ntinyint { get; set; }
        public short nsmallint { get; set; }
        public int nint { get; set; }
        public long nbigint { get; set; }
        public float nreal { get; set; }
        public double nfloat { get; set; }
        public decimal nmoney { get; set; }
        public decimal nsmallmoney { get; set; }
        public DateTime nsmalldatetime { get; set; }
        public DateTime ndatetime { get; set; }
        public Guid nuniqueidentifier { get; set; }
        public byte[] nbinary { get; set; }
        public byte[] nvarbinary { get; set; }
        public string nchar { get; set; }
        public string nvarchar { get; set; }
        public string nnchar { get; set; }
        public string nnvarchar { get; set; }
        public string nXML { get; set; }
        public decimal ndecimal4 { get; set; }
        public decimal ndecimal8 { get; set; }
        public decimal ndecimal12 { get; set; }
        public string ntext { get; set; }
        public string nntext { get; set; }
        public byte[] nimage { get; set; }
        public byte[] nvarbinarysmall { get; set; }
        public byte[] ntimestamp { get; set; }
    }
}