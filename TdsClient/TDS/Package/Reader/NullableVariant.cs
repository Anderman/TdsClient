using System.Text;
using Medella.TdsClient.Constants;
using Medella.TdsClient.TDS.Row.Reader.StringHelpers;

namespace Medella.TdsClient.TDS.Package.Reader
{
    public partial class TdsPackageReader
    {
        public object? ReadNullableSqlVariant(int index)
        {
            var lenTotal = (int?)ReadLengthNullableData(index);
            // get the SQLVariant type
            if (lenTotal == null) return null;
            var type = ReadByte();
            // read cbPropBytes
            var cbPropsActual = ReadByte();

            var lenConsumed = TdsEnums.SQLVARIANT_SIZE + cbPropsActual; // type, count of propBytes, and actual propBytes
            var lenData = (int)lenTotal - lenConsumed; // length of actual data

            // read known properties and skip unknown properties

            //
            // now read the value
            //
            switch (type)
            {
                case TdsEnums.SQLBIT:
                    return ReadByte() != 0;
                case TdsEnums.SQLINT1:
                    return ReadByte();
                case TdsEnums.SQLINT2:
                    return ReadInt16();
                case TdsEnums.SQLINT4:
                    return ReadInt32();
                case TdsEnums.SQLINT8:
                    return ReadInt64();
                case TdsEnums.SQLFLT4:
                    return ReadFloat();
                case TdsEnums.SQLFLT8:
                    return ReadDouble();
                case TdsEnums.SQLMONEY:
                    return ReadSqlMoney(8);
                case TdsEnums.SQLMONEY4:
                    return ReadSqlMoney(4);
                case TdsEnums.SQLDATETIM4:
                    return ReadSqlDateTime(4);
                case TdsEnums.SQLDATETIME:
                    return ReadSqlDateTime(8);
                case TdsEnums.SQLUNIQUEID:
                    return ReadGuid();
                case TdsEnums.SQLDECIMALN:
                case TdsEnums.SQLNUMERICN:
                {
                    var precision = ReadByte();
                    var scale = ReadByte();
                    return ReadSqlDecimal(lenData, scale);
                }

                case TdsEnums.SQLBIGBINARY:
                case TdsEnums.SQLBIGVARBINARY:
                {
                    var lenMax = ReadUInt16();
                    return ReadByteArray(new byte[lenData], 0, lenData);
                }
                case TdsEnums.SQLBIGCHAR:
                case TdsEnums.SQLBIGVARCHAR:
                {
                    var collation = ReadCollation();
                    var lenMax = ReadUInt16();
                    var encoding = Encoding.GetEncoding(collation.GetCodePage());
                    return ReadString(encoding, lenData);
                }
                case TdsEnums.SQLNCHAR:
                case TdsEnums.SQLNVARCHAR:
                {
                    var collation = ReadCollation();
                    var lenMax = ReadUInt16();
                    return ReadUnicodeChars(lenData);
                }
                case TdsEnums.SQLDATE:
                    return ReadSqlDate();
                case TdsEnums.SQLTIME:
                {
                    var scale = ReadByte();
                    return ReadSqlTime(lenData, scale);
                }
                case TdsEnums.SQLDATETIME2:
                {
                    var scale = ReadByte();
                    return ReadSqlDateTime(lenData, scale);
                }
                case TdsEnums.SQLDATETIMEOFFSET:
                {
                    var scale = ReadByte();
                    return ReadSqlDateTimeOffset(lenData, scale);
                }
            }

            return null;
        }
    }
}