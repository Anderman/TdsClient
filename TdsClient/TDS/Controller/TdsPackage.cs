using Medella.TdsClient.TdsStream;
using Medella.TdsClient.TDS.Package.Reader;
using Medella.TdsClient.TDS.Package.Writer;

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