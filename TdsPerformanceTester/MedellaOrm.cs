using System.Linq;
using Medella.TdsClient.TDS;
using Medella.TdsClient.TDS.Processes;

namespace TdsPerformanceTester
{
    public class MedellaOrm 
    {
        private readonly TdsConnection _tds;
        private int i = 1;
        private const string ConnectionString = @"Server=127.0.0.1;Database=tempdb;Trusted_Connection=True;";

        public MedellaOrm()
        {
            _tds = Tds.GetConnection(ConnectionString);
        }

        public void Run()
        {
            if (i++ > 5000)
                i = 1;

            //using (var tds = Tds.GetConnection(ConnectionString))
            {
                var x= _tds.ExecuteQuery<Post>($@"select * from Posts where Id = 1");
            }
        }
    }
}