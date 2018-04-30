using System.Text;
using Medella.TdsClient.TDS.Reader.StringHelpers;

namespace Medella.TdsClient.TDS.Package
{
    public class TdsSession
    {
        public int DefaultCodePage { get; set; }
        public Encoding DefaultEncoding { get; set; }
        public SqlCollations DefaultCollation { get; set; }
    }
}