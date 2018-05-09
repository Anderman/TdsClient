using Medella.TdsClient.SNI;
using Medella.TdsClient.SNI.SniNp;
using Medella.TdsClient.TDS.Package;

namespace Medella.TdsClient.TDS.Controller
{
    public class TdsPackage
    {
        public TdsPackageReader Reader;
        public TdsPackageWriter Writer;

        public TdsPackage(ISniHandle tdsStream)
        {
            Reader = new TdsPackageReader(tdsStream);
            Writer = new TdsPackageWriter(tdsStream);
        }
    }
}