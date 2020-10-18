using System.Collections.Generic;
using System.Data;

namespace Medella.TdsClient.Constants
{
    public static class TdsMetaType
    {
        public static readonly MetaDataWrite SqlFlt4 = new MetaDataWrite { HasScale = false, HasPrecision = false, HasCollation = false, IsLong = false, IsPlp = false, NullableType = TdsEnums.SQLFLTN };
        public static readonly MetaDataWrite SqlFlt8 = new MetaDataWrite { HasScale = false, HasPrecision = false, HasCollation = false, IsLong = false, IsPlp = false, NullableType = TdsEnums.SQLFLTN };
        public static readonly MetaDataWrite SqlInt1 = new MetaDataWrite { HasScale = false, HasPrecision = false, HasCollation = false, IsLong = false, IsPlp = false, NullableType = TdsEnums.SQLINTN };
        public static readonly MetaDataWrite SqlInt2 = new MetaDataWrite { HasScale = false, HasPrecision = false, HasCollation = false, IsLong = false, IsPlp = false, NullableType = TdsEnums.SQLINTN };
        public static readonly MetaDataWrite SqlInt3 = new MetaDataWrite { HasScale = false, HasPrecision = false, HasCollation = false, IsLong = false, IsPlp = false, NullableType = TdsEnums.SQLINTN };
        public static readonly MetaDataWrite SqlInt4 = new MetaDataWrite { HasScale = false, HasPrecision = false, HasCollation = false, IsLong = false, IsPlp = false, NullableType = TdsEnums.SQLINTN };
        public static readonly MetaDataWrite SqlDateTimN = new MetaDataWrite { HasScale = false, HasPrecision = false, HasCollation = false, IsLong = false, IsPlp = false, NullableType = TdsEnums.SQLDATETIMN };
        public static readonly MetaDataWrite SqlNVarChar = new MetaDataWrite { HasScale = false, HasPrecision = false, HasCollation = true, IsLong = false, IsPlp = false, NullableType = TdsEnums.SQLNVARCHAR };
        public static readonly MetaDataWrite SqlNumericN = new MetaDataWrite { HasScale = true, HasPrecision = true, HasCollation = false, IsLong = false, IsPlp = false, NullableType = TdsEnums.SQLNUMERICN };
        public static readonly MetaDataWrite SqlBitN = new MetaDataWrite { HasScale = false, HasPrecision = false, HasCollation = false, IsLong = false, IsPlp = false, NullableType = TdsEnums.SQLBITN };
        public static MetaDataRead[] TdsTypes = new MetaDataRead[256];


        static TdsMetaType()
        {
            foreach (var metaType in MetaDataRead.Lookup)
                TdsTypes[metaType.Key] = metaType.Value;
        }

        public class MetaDataWrite
        {
            public bool IsLong { get; set; }
            public bool IsPlp { get; set; }
            public bool HasScale { get; set; }
            public bool HasPrecision { get; set; }
            public bool HasCollation { get; set; }
            public int NullableType { get; set; }
        }

        public class MetaDataRead
        {
            internal static readonly Dictionary<int, MetaDataRead> Lookup = new Dictionary<int, MetaDataRead>
            {
                { TdsEnums.SQLBIT, new MetaDataRead { LenBytes = 0, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Bit } },
                { TdsEnums.SQLBITN, new MetaDataRead { LenBytes = 1, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Bit } },
                { TdsEnums.SQLINT1, new MetaDataRead { LenBytes = 0, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.TinyInt } },
                { TdsEnums.SQLINT2, new MetaDataRead { LenBytes = 0, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.SmallInt } },
                { TdsEnums.SQLINT4, new MetaDataRead { LenBytes = 0, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Int } },
                { TdsEnums.SQLINT8, new MetaDataRead { LenBytes = 0, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.BigInt } },
                { TdsEnums.SQLINTN, new MetaDataRead { LenBytes = 1, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.TinyInt } },
                { TdsEnums.SQLMONEY4, new MetaDataRead { LenBytes = 0, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.SmallMoney } },
                { TdsEnums.SQLMONEY, new MetaDataRead { LenBytes = 0, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Money } },
                { TdsEnums.SQLMONEYN, new MetaDataRead { LenBytes = 1, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Money } },
                { TdsEnums.SQLFLT4, new MetaDataRead { LenBytes = 0, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Real } },
                { TdsEnums.SQLFLT8, new MetaDataRead { LenBytes = 0, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Float } },
                { TdsEnums.SQLFLTN, new MetaDataRead { LenBytes = 1, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Float } },
                { TdsEnums.SQLDATETIME2, new MetaDataRead { LenBytes = 0, HasScale = true, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.DateTime2 } },
                { TdsEnums.SQLDATETIMEOFFSET, new MetaDataRead { LenBytes = 0, HasScale = true, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.DateTimeOffset } },
                { TdsEnums.SQLDATETIM4, new MetaDataRead { LenBytes = 0, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.SmallDateTime } },
                { TdsEnums.SQLDATETIME, new MetaDataRead { LenBytes = 0, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.DateTime } },
                { TdsEnums.SQLDATETIMN, new MetaDataRead { LenBytes = 1, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.DateTime } },
                { TdsEnums.SQLDATE, new MetaDataRead { LenBytes = 0, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Date } },
                { TdsEnums.SQLTIME, new MetaDataRead { LenBytes = 0, HasScale = true, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Time } },
                { TdsEnums.SQLDECIMALN, new MetaDataRead { LenBytes = 1, HasScale = true, HasPrecision = true, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Decimal } },
                { TdsEnums.SQLUNIQUEID, new MetaDataRead { LenBytes = 1, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.UniqueIdentifier } },
                { TdsEnums.SQLBIGBINARY, new MetaDataRead { LenBytes = 2, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Binary } },
                { TdsEnums.SQLBIGVARBINARY, new MetaDataRead { LenBytes = 2, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.VarBinary } },
                { TdsEnums.SQLBIGCHAR, new MetaDataRead { LenBytes = 2, HasScale = false, HasPrecision = false, HasCollation = true, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Char } },
                { TdsEnums.SQLBIGVARCHAR, new MetaDataRead { LenBytes = 2, HasScale = false, HasPrecision = false, HasCollation = true, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.VarChar } },
                { TdsEnums.SQLNCHAR, new MetaDataRead { LenBytes = 2, HasScale = false, HasPrecision = false, HasCollation = true, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.NChar } },
                { TdsEnums.SQLNVARCHAR, new MetaDataRead { LenBytes = 2, HasScale = false, HasPrecision = false, HasCollation = true, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.NVarChar } },
                { TdsEnums.SQLUDT, new MetaDataRead { LenBytes = 2, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = true, SqlDbType = SqlDbType.Udt } },
                { TdsEnums.SQLVARIANT, new MetaDataRead { LenBytes = 4, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Variant } },
                { TdsEnums.SQLIMAGE, new MetaDataRead { LenBytes = 4, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = true, IsPlp = false, SqlDbType = SqlDbType.Image } },
                { TdsEnums.SQLTEXT, new MetaDataRead { LenBytes = 4, HasScale = false, HasPrecision = false, HasCollation = true, IsTextOrImage = true, IsPlp = false, SqlDbType = SqlDbType.Text } },
                { TdsEnums.SQLNTEXT, new MetaDataRead { LenBytes = 4, HasScale = false, HasPrecision = false, HasCollation = true, IsTextOrImage = true, IsPlp = false, SqlDbType = SqlDbType.NText } },
                { TdsEnums.SQLXMLTYPE, new MetaDataRead { LenBytes = 0, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = true, SqlDbType = SqlDbType.Xml } },

                { TdsEnums.SQLCHAR, new MetaDataRead { LenBytes = 1, HasScale = false, HasPrecision = false, HasCollation = true, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Char } },
                { TdsEnums.SQLVARCHAR, new MetaDataRead { LenBytes = 1, HasScale = false, HasPrecision = false, HasCollation = true, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.VarChar } },
                { TdsEnums.SQLNUMERICN, new MetaDataRead { LenBytes = 1, HasScale = true, HasPrecision = true, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Decimal } },
                { TdsEnums.SQLBINARY, new MetaDataRead { LenBytes = 1, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.Binary } },
                { TdsEnums.SQLVARBINARY, new MetaDataRead { LenBytes = 1, HasScale = false, HasPrecision = false, HasCollation = false, IsTextOrImage = false, IsPlp = false, SqlDbType = SqlDbType.VarBinary } }
            };

            public SqlDbType SqlDbType { get; set; }
            public bool IsTextOrImage { get; set; }
            public bool IsPlp { get; set; }
            public bool HasScale { get; set; }
            public bool HasPrecision { get; set; }
            public bool HasCollation { get; set; }

            public int LenBytes { get; set; }
        }
    }
}