using System;
using System.Diagnostics;

namespace TdsPerformanceTester
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            OrmTester.EnsureDBSetup();
            var bm = new Benchmarks(Console.WriteLine);
            bm.Run();
        }
    }
}

