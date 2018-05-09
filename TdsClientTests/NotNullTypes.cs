using System;
using ServerConnectionOptions = TdsClientTests.ServerConnectionOptions;

namespace TdsClientTests
{
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