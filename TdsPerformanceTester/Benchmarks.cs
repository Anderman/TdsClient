using System;
using System.Diagnostics;

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
            var orm1 = new MedellaOrm();
            orm1.Run();
            //test
            for (var i = 0; i < 10; i++) MedellaOrm();
        }

        private void MedellaOrm()
        {
            var orm = new MedellaOrm();
            var iteration = 5;
            using (var sw = new AutoStopWatch($"{nameof(MedellaOrm),-20} {{0}}", iteration, _reporter))
            {
                while (sw.IsRunnning)
                    for (var j = 0; j < iteration; j++)
                        orm.Run();
            }
        }
    }
}