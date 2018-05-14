using System;
using Medella.TdsClient.Contants;

namespace Medella.TdsClient.TDS.Controller
{
    public class TdsParameter
    {
        public TdsParameter(string name, decimal value, byte? scale = null)
        {
            Name = name; Value = value; Size = 17;
            Scale = (byte)(scale ?? (decimal.GetBits(value)[3] >> 16) & 0xff);
            MetaData = TdsMetaType.SqlNumericN;
            SqlName = $"decimal(28,{Scale})";
        }
        public TdsParameter(string name, DateTime value) { Name = name; Value = value; Size = 8; MetaData = TdsMetaType.SqlDateTimN; SqlName = $"datetime"; }
        public TdsParameter(string name, string value) { Name = name; Value = value; Size = value.Length * 2; MetaData = TdsMetaType.SqlNVarChar; SqlName = $"nvarchar({value.Length})"; }
        public TdsParameter(string name, bool value) { Name = name; Value = value; Size = 1; MetaData = TdsMetaType.SqlBitN; SqlName = "bit"; }
        public TdsParameter(string name, byte value) { Name = name; Value = value; Size = 1; MetaData = TdsMetaType.SqlInt1; SqlName = "tinyint"; }
        public TdsParameter(string name, short value) { Name = name; Value = value; Size = 2; MetaData = TdsMetaType.SqlInt2; SqlName = "smallint"; }
        public TdsParameter(string name, int value) { Name = name; Value = value; Size = 4; MetaData = TdsMetaType.SqlInt3; SqlName = "int"; }
        public TdsParameter(string name, long value) { Name = name; Value = value; Size = 8; MetaData = TdsMetaType.SqlInt4; SqlName = "bigint"; }
        public TdsParameter(string name, float value) { Name = name; Value = value; Size = 4; MetaData = TdsMetaType.SqlFlt4; SqlName = "real"; }
        public TdsParameter(string name, double value) { Name = name; Value = value; Size = 8; MetaData = TdsMetaType.SqlFlt8; SqlName = "float"; }

        public string Name;
        public object Value;
        public int? Precision;
        public byte Scale;
        public TdsMetaType.MetaDataWrite MetaData;
        public int Size { get; set; }
        public int Status { get; set; }
        public bool IsNull => Value == null;

        public string SqlName;

    }
}