using Medella.TdsClient.TdsStream.Sspi;
using Xunit;

namespace TdsClientTests.Sni.Sspi
{
    public class SspiHelperTest
    {
        [Fact]
        public void TestManageSspiWrapper()
        {
            var x = new SspiHelper("");
            var clientToken = x.CreateClientToken(null);
            Assert.NotEmpty(clientToken);
        }
    }
}