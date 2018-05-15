using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace TdsPerformanceTester
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var bm = new Benchmarks(Console.WriteLine);
            bm.Run().GetAwaiter().GetResult();
        }
    }
}

