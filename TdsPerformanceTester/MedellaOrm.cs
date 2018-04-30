using System.Linq;
using Medella.TdsClient.TDS;

namespace TdsPerformanceTester
{
    public class MedellaOrm 
    {
        private readonly Tds _tds;
        private int i = 1;
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=test;Trusted_Connection=True;";

        public MedellaOrm()
        {
            _tds = new Tds(ConnectionString);
            _tds.Connect();
            _tds.Login();
        }

        public void Run()
        {
            if (i++ > 5000)
                i = 1;

            var x=_tds.ExecuteQuery<Post>($"select * from Posts where id={i}").ToArray();
        }
    }
}