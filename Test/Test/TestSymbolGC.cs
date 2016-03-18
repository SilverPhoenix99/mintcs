using Mint;
using System;
using System.Diagnostics;

namespace Test
{
    static class TestSymbolGC
    {
        static void Test()
        {
            var r = new Random(345757345);
            for(int j = 0; j < 100; j++)
            {
                var s = "";

                for(int i = 0; i < 1000; i++)
                {
                    s += r.Next() + " ";
                }

                new Symbol(s);
            }

            GC.Collect();

            Debug.Assert(Symbol.Count < 100);
        }
    }
}
