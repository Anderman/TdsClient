using System.Linq;
using System.Threading.Tasks;
using Medella.TdsClient.TDS;
using Medella.TdsClient.TDS.Processes;
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
        public async Task can_upload_int_Column()
        {
            var size = 5531;
            var obj = new TestBulkcopy[size];
            for (int i = 0; i < size; i++)
            {
                obj[i] = new TestBulkcopy { Id = i, Id1 = i + 1 };
            }
            var cnn = TdsConnectionPools.GetConnectionPool(ConnectionString);
            await cnn.ExecuteNonQueryAsync("if OBJECT_ID('bulkcopy') is not null DROP TABLE bulkcopy CREATE TABLE bulkcopy (Id int, Id1 bigint) ");
            await cnn.ExecuteNonQueryAsync("if OBJECT_ID('bulkcopy2') is not null DROP TABLE bulkcopy2 CREATE TABLE bulkcopy2 (Id int, Id1 bigint) ");
            await cnn.BulkInsertAsync(obj.ToList(), "bulkcopy");
            //var task2 = cnn.BulkInsertAsync(obj.ToList(), "bulkcopy2");
            //var task3 = cnn.BulkInsertAsync(obj.ToList(), "bulkcopy");
            //Task.WaitAll(task1, task2, task3);
        }

        public class TestBulkcopy
        {
            public int? Id { get; set; }
            public long? Id1 { get; set; }
        }
    }
}