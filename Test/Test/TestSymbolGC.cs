using System;
using System.Diagnostics;

namespace Mint.Test
{
    internal static class TestSymbolGC
    {
        private static void Test()
        {
            var r = new Random(345757345);
            for(var j = 0; j < 100; j++)
            {
                var s = "";

                for(var i = 0; i < 1000; i++)
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
