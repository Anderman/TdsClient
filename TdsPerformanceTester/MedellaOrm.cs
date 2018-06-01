using System.Threading.Tasks;
using Medella.TdsClient.TDS;
using Medella.TdsClient.TDS.Controller;
using Medella.TdsClient.TDS.Processes;

namespace TdsPerformanceTester
{
    public class MedellaOrm
    {
        private readonly string _connectionString = Program.ConnectionString;
        private readonly TdsConnectionPool _tds;
        private const int conc = 4;
        private int i = 1;

        public MedellaOrm()
        {
            var connections = new TdsConnection[conc];
            _tds = TdsConnectionPools.GetConnectionPool(_connectionString);
            var task = Parallel.For(0, conc, (x) => connections[x]=_tds.GetConnection());
            //OrmTester.EnsureDbSetup(_connectionString);
            task= Parallel.For(0, conc, (x) => _tds.Return(connections[x]));
        }

        public void Run()
        {
            if (i++ > 5000)
                i = 1;

            //using (var tds = TdsConnectionPools.GetConnectionPool(ConnectionString))
            {
                var task = Parallel.For(0, conc,
                    ctr => _tds.ExecuteQuery<Post>($@"select Id,CreationDate,LastChangeDate, Counter1, Counter2, Counter3, Counter4, Counter5, Counter6, Counter7, Counter8, Counter9 from Posts"));
            }
        }
    }
}