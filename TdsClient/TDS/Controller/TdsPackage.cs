using Medella.TdsClient.TDS.Package.Reader;
using Medella.TdsClient.TDS.Package.Writer;
using Medella.TdsClient.TdsStream;

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