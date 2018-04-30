using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Reader.StringHelpers;

namespace Medella.TdsClient.TDS.Messages.Server
{
    public static class ParserCommonExtentions
    {
        public static SqlCollations ReadCollation(this TdsPackageReader tdsPackageReader)
        {
            return new SqlCollations //do we realy need a new collation or can we use an existing
            {
                Info = tdsPackageReader.ReadUInt32(),
                SortId = tdsPackageReader.ReadByte()
            };
        }
    }
}