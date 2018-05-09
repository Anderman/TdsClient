using Medella.TdsClient.TDS;
using Xunit;

namespace TdsClientTests
{
    public class BulkLoadTest
    {
        //Data read with SELECT * FROM bulkcopy
        //04 01 00 3F 00 38 01 00
        //FD 01 00 B9 00 00 00 00 00 00 00 00 00 DOne, status=0001, cmd=00B9, count=0x0000_0000
        //81 01 00 colmetadata 0x001 column
        //00 00 00 00 //usertype ignore
        //09 00//2 flags
        //26 04 //Intn 4 lang
        //02 49 00 64 00 2string Id
        //FD 11 00 C1 00 00 00 00 00 00 00 00 00 DOne, status=0011, cmd=00C1, count=0x0000_0000
        //FD 00 00 BA 00 00 00 00 00 00 00 00 00 DOne, status=0000, cmd=00BA, count=0x0000_0000

        //Coldata writen by bulkcopy
        //0x07,0x01,0x00,0x27,0x00,0x00,0x01,0x00, //bulkcopy, status=0x01, len=0x0027,  packagenumber=0x1
        //0x81,0x01,0x00,//colmetadata 0x001 column
        //0x00,0x00,0x00,0x00,//usertype ignore
        //0x09,0x00, //flags
        //0x26,0x04, //int 4
        //0x02,0x49,0x00,0x64,0x00, //2 bytes ID
        //row data
        //0xD1,0x04,0x02,0x00,0x00,0x00, datarow 4 bytes value=0x0000_0002
        //0xFD,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=test;Trusted_Connection=True;";

        [Fact]
        public void can_upload_int_Column()
        {
            var cnn = Tds.GetConnection(ConnectionString);
            cnn.BulkInsert();
        }
    }
}