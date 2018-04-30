using System.Data;
using System.Text;

namespace Medella.TdsClient.TDS.Messages.Server.Internal
{
    public sealed class ColumnMetadata 
    {
        public byte TdsType { get; set; }
        public string Column { get; set; }
        public bool IsColumnSet { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsNullable { get; set; }
        public bool IsPlp { get; set; }
        public bool IsLong { get; set; }
        public byte Updatability { get; set; }
        public byte Scale { get; set; }
        public MultiPartTableName PartTableName { get; set; }
        public Encoding Encoding { get; set; }
        public SqlDbType SqlDbType { get; set; }

        //// Xml specific metadata
        public string XmlSchemaCollectionDatabase { get; set; }
        public string XmlSchemaCollectionName { get; set; }
        public string XmlSchemaCollectionOwningSchema { get; set; }

        
    }
}