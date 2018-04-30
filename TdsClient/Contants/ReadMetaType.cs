using System.Collections.Generic;
using System.Data;


namespace Medella.TdsClient.Contants
{
    public class ReadMetaType
    {
        public SqlDbType SqlDbType { get; set; }
        public bool IsLong { get; set; }
        public bool IsPlp { get; set; }
        public bool HasScale { get; set; }
        public bool HasPrecision { get; set; }
        public bool HasCollation { get; set; }


        private static readonly Dictionary<int, ReadMetaType> Lookup = new Dictionary<int, ReadMetaType>()
        {
            {TdsEnums.SQLMONEYN                 , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Money                }},
            {TdsEnums.SQLDATETIMN               , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.DateTime             }},
            {TdsEnums.SQLINTN                   , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.TinyInt              }},
            {TdsEnums.SQLFLTN                   , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Float                }},
            {TdsEnums.SQLBINARY                 , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Binary               }},
            {TdsEnums.SQLBIGBINARY              , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Binary               }},
            {TdsEnums.SQLBIGVARBINARY           , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.VarBinary            }},
            {TdsEnums.SQLVARCHAR                , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = true , IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.VarChar              }},
            {TdsEnums.SQLCHAR                   , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = true , IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Char                 }},
            {TdsEnums.SQLBIGCHAR                , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = true , IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Char                 }},
            {TdsEnums.SQLBIT                    , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Bit                  }},
            {TdsEnums.SQLBITN                   , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Bit                  }},
            {TdsEnums.SQLDECIMALN               , new ReadMetaType {HasScale = true , HasPrecision = true  , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Decimal              }},
            {TdsEnums.SQLNUMERICN               , new ReadMetaType {HasScale = true , HasPrecision = true  , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Decimal              }},
            {TdsEnums.SQLUNIQUEID               , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.UniqueIdentifier     }},
            {TdsEnums.SQLNCHAR                  , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = true , IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.NChar                }},
            {TdsEnums.SQLVARIANT                , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Variant              }},
            {TdsEnums.SQLDATE                   , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Date                 }},
            {TdsEnums.SQLTIME                   , new ReadMetaType {HasScale = true , HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Time                 }},
            {TdsEnums.SQLDATETIME2              , new ReadMetaType {HasScale = true , HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.DateTime2            }},
            {TdsEnums.SQLDATETIMEOFFSET         , new ReadMetaType {HasScale = true , HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.DateTimeOffset       }},
            {TdsEnums.SQLINT1                   , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.TinyInt              }},
            {TdsEnums.SQLINT2                   , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.SmallInt             }},
            {TdsEnums.SQLINT4                   , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Int                  }},
            {TdsEnums.SQLINT8                   , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.BigInt               }},
            {TdsEnums.SQLMONEY4                 , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.SmallMoney           }},
            {TdsEnums.SQLMONEY                  , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Money                }},
            {TdsEnums.SQLDATETIM4               , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.SmallDateTime        }},
            {TdsEnums.SQLDATETIME               , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.DateTime             }},
            {TdsEnums.SQLFLT8                   , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Float                }},
            {TdsEnums.SQLFLT4                   , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.Real                 }},
            {TdsEnums.SQLVARBINARY              , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = false,  IsPlp = false,  SqlDbType = SqlDbType.VarBinary            }},
            {TdsEnums.SQLBIGVARCHAR             , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = true , IsLong = true,   IsPlp = true,   SqlDbType = SqlDbType.VarChar              }},
            {TdsEnums.SQLNVARCHAR               , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = true , IsLong = true,   IsPlp = true,   SqlDbType = SqlDbType.NVarChar             }},
            {TdsEnums.SQLXMLTYPE                , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = true,   IsPlp = true,   SqlDbType = SqlDbType.Xml                  }},
            {TdsEnums.SQLNTEXT                  , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = true , IsLong = true,   IsPlp = false,  SqlDbType = SqlDbType.NText                }},
            {TdsEnums.SQLTEXT                   , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = true , IsLong = true,   IsPlp = false,  SqlDbType = SqlDbType.Text                 }},
            {TdsEnums.SQLIMAGE                  , new ReadMetaType {HasScale = false, HasPrecision = false , HasCollation = false, IsLong = true,   IsPlp = false,  SqlDbType = SqlDbType.Image                }},
        };

        public static ReadMetaType[] TdsMetaTypeRead = new ReadMetaType[256];

        static ReadMetaType()
        {
            foreach (var metaType in Lookup)
                TdsMetaTypeRead[metaType.Key] = metaType.Value;
        }
    }
}