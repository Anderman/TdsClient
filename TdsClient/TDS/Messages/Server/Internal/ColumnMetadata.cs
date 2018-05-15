using System.Data;
using System.Text;
using Medella.TdsClient.Contants;

namespace Medella.TdsClient.TDS.Messages.Server.Internal
{
    public sealed class ColumnMetadata 
    {
        public byte Flag1 { get; set; }
        public byte Flag2 { get; set; }
        public byte TdsType { get; set; }
        public byte Scale { get; set; }
        public string Column { get; set; }
        public bool IsTextOrImage { get; set; }
        public bool IsPlp { get; set; }
        public Encoding Encoding { get; set; }
        public byte Updatability => (byte) ((Flag1 & TdsEnums.Updatability) >> 2);
        public bool IsIdentity => TdsEnums.Identity == (Flag1 & TdsEnums.Identity);
        public bool IsNullable => TdsEnums.Nullable == (Flag1 & TdsEnums.Nullable);
        public bool IsColumnSet => TdsEnums.IsColumnSet == (Flag2 & TdsEnums.IsColumnSet);
        public TdsMetaType.MetaDataRead MetaType { get; set; }
    }
}