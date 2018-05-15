using Medella.TdsClient.TdsStream;
using Medella.TdsClient.TDS.Package;
using Medella.TdsClient.TDS.Package.Reader;
using TdsPackageWriter = Medella.TdsClient.TDS.Package.Writer.TdsPackageWriter;

namespace Medella.TdsClient.TDS.Controller
{
    public class TdsPackage
    {
        public TdsPackageReader Reader;
        public TdsPackageWriter Writer;

        public TdsPackage(ITdsStream tdsStream)
        {
            Reader = new TdsPackageReader(tdsStream);
            Writer = new TdsPackageWriter(tdsStream);
        }
    }
}