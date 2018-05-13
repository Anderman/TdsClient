using System;
using System.Diagnostics;
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
        public async Task Run()
        {
            //run static initializers
            var orm1 = new MedellaOrm();
            orm1.Run();
            //orm1.Run();
            //test
            for (var i = 0; i < 5; i++) HandcodedOrm();
            for (var i = 0; i < 5; i++) MedellaOrm();
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