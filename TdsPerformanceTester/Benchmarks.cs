using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TdsPerformanceTester
{
    public class Benchmarks
    {
        private readonly Action<string> _reporter;
        private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=test;Password=1;user=sa;";

        public Benchmarks(Action<string> reporter)
        {
            _reporter = reporter;
        }
        public async Task Run()
        {
            //run static initializers
            var orm1 = new MedellaOrm();
            orm1.Run();
            //test
            //for (var i = 0; i < 5; i++) sqlOpen();
            for (var i = 0; i < 5; i++) MedellaOrm();
        }

        public void sqlOpen()
        {
            const int iteration = 10;
            using (var sw = new AutoStopWatch($"{nameof(sqlOpen),-20} {{0}}", iteration, _reporter))
            {
                while (sw.IsRunnning)
                    for (var j = 0; j < iteration; j++)
                    {
                        using (var cnn=new SqlConnection(ConnectionString))
                            cnn.Open();
                    }
            }

        }

        private void MedellaOrm()
        {
            var orm = new MedellaOrm();
            const int iteration = 1;
            using (var sw = new AutoStopWatch($"{nameof(MedellaOrm),-20} {{0}}", iteration, _reporter))
            {
                while (sw.IsRunnning)
                    for (var j = 0; j < iteration; j++)
                        orm.Run();
            }
        }
        private void HandcodedOrm()
        {
            var orm = new HandCodedOrm();
            const int iteration = 1;
            using (var sw = new AutoStopWatch($"{nameof(HandcodedOrm),-20} {{0}}", iteration, _reporter))
            {
                while (sw.IsRunnning)
                    for (var j = 0; j < iteration; j++)
                        orm.Run();
            }
        }
    }
}