using Mint;
using Mint.Binding;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Test
{
    internal static class Program
    {
        public static bool InVisualStudio => Environment.GetEnvironmentVariable("VisualStudioVersion") != null;

        public static void Main(string[] args)
        {
            Debug.Assert(Marshal.SizeOf(typeof(NilClass))   <= sizeof(bool));
            Debug.Assert(Marshal.SizeOf(typeof(TrueClass))  <= sizeof(bool));
            Debug.Assert(Marshal.SizeOf(typeof(FalseClass)) <= sizeof(bool));
            Debug.Assert(Marshal.SizeOf(typeof(Fixnum))     <= sizeof(long));
            Debug.Assert(Marshal.SizeOf(typeof(Symbol))     <= IntPtr.Size);

            Repl.Run();
        }
    }
}
