using System.Reflection;
using System.Text;
using Medella.TdsClient.Contants;
using Medella.TdsClient.TDS.Row.Reader.StringHelpers;

namespace Medella.TdsClient.TDS.Messages.Server.Internal
{
    public sealed class MetadataBulkCopy 
    {
        public byte Flag1 { get; set; }
        public byte Flag2 { get; set; }
        public byte TdsType { get; set; }
        public int Length { get; set; }
        public byte Precision { get; set; }
        public byte Scale { get; set; }
        public string Column { get; set; }
        public bool IsTextOrImage { get; set; }
        public bool IsPlp { get; set; }
        public TdsMetaType.MetaDataRead MetaType { get; set; }
        public MultiPartTableName PartTableName { get; set; }
        public bool IsIdentity => TdsEnums.Identity == (Flag1 & TdsEnums.Identity);
        public XmlSchema XmlSchema { get; set; }
        public Udt Udt { get; set; }
        public SqlCollations Collation { get; set; }
        public Encoding Encoding { get; set; }
        public bool NonUniCode => TdsType == TdsEnums.SQLBIGCHAR || TdsType == TdsEnums.SQLBIGVARCHAR || TdsType == TdsEnums.SQLTEXT;
        public PropertyInfo PropertyInfo { get; set; }
    }
}