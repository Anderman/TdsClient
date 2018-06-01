using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace TdsPerformanceTester
{
    public class Benchmarks
    {
        private readonly Action<string> _reporter;

        public Benchmarks(Action<string> reporter)
        {
            _reporter = reporter;
        }

        public void Run()
        {
            //run static initializers
            //var orm1 = new MedellaOrm();
            //orm1.Run();
            //test
            //for (var i = 0; i < 5; i++) sqlOpen();
            for (var i = 0; i < 10; i++) MedellaOrm();
            for (var i = 0; i < 10; i++) HandcodedOrm();
        }

        public void sqlOpen()
        {
            const int iteration = 10;
            using (var sw = new AutoStopWatch($"{nameof(sqlOpen),-20} {{0}}", iteration, _reporter))
            {
                while (sw.IsRunnning)
                    for (var j = 0; j < iteration; j++)
                        using (var cnn = new SqlConnection(Program.ConnectionString))
                        {
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
            var orm = new HandCodedOrmPosts();
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