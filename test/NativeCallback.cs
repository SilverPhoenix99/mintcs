using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace mint.test
{
    static class NativeCallback
    {
        [DllImport("test.so", CallingConvention = CallingConvention.Cdecl)]
        static extern void testCallback(Action callback);

        [DllImport("test.so", CallingConvention = CallingConvention.Cdecl)]
        static extern void saveCallback(Action callback);

        [DllImport("test.so", CallingConvention = CallingConvention.Cdecl)]
        static extern void testSavedCallback();

        static readonly Action callback = Hello;

        static void Hello()
        {
            Console.WriteLine("Hello C world");
        }

        public static void Test()
        {
            //testCallback(callback);

            saveCallback(callback);

            var r = new Random();
            int[] arr;
            for(int i = 0; i < 5; i++)
            {
                arr = new int[r.Next(100)];
            }

            arr = null;
            r = null;

            for(int i = 0; i < 5; i++) GC.Collect();

            testSavedCallback();
        }
    }
}
