using System;
using System.Runtime.InteropServices;

namespace Mint.Test
{
    internal static class NativeCallback
    {
        [DllImport("test.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern void testCallback(Action callback);

        [DllImport("test.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern void saveCallback(Action callback);

        [DllImport("test.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern void testSavedCallback();

        private static readonly Action CALLBACK = Hello;

        private static void Hello()
        {
            Console.WriteLine("Hello C world");
        }

        public static void Test()
        {
            //testCallback(callback);

            saveCallback(CALLBACK);

            var r = new Random();
            int[] arr;
            for(var i = 0; i < 5; i++)
            {
                arr = new int[r.Next(100)];
            }

            arr = null;
            r = null;

            for(var i = 0; i < 5; i++) GC.Collect();

            testSavedCallback();
        }
    }
}
