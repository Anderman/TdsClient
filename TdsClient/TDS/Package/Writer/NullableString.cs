using System.Text;

// ReSharper disable once CheckNamespace
namespace Medella.TdsClient.TDS.Package.Writer
{
    public partial class TdsPackageWriter
    {
        public void WriteNullableSqlString(string value, int index)
        {
            if (value == null)
            {
                WriteNullableSqlBinary(null, index);
                return;
            }

            var md = ColumnsMetadata[index];
            if (md.NonUniCode)
                WriteNullableSqlBinary(md.Encoding!.GetBytes(value), index);
            else
                WriteNullableSqlBinary(Encoding.Unicode.GetBytes(value), index);
        }
    }
}