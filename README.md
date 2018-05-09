# TdsClient

A Rewrite of SqlClient with improved performance and memory footprint. A cleaned library with is suitable for DI.

Goal a new lib. without connectionstring and SqlDataReader, 

Todo:
* BulkInsert
* Connection pooling
* Async
* Create a TdsStream from connection settings and inject this stream in the TdsClient. This will improve the testing and made it possible to 
inject a TestTdsStream. Which will also lead to easy performance testing
* Get rid of a lot a internal corefx classes needed for sspi and sni
* cleanup

The connectionstring could be a normal Json settings.

## Current status.
Execute queries and read a single result set into a poco class. Tested all sqltypes and SqlVariant types

```
          var tds = new Tds(ConnectionString);
            tds.Connect();
            tds.Login();
            var x = tds.ExecuteQuery<NotNullTypes>(TestStatements.NotNullTable);

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
