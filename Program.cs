using mint.types;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

using Ex = System.Linq.Expressions.Expression;

namespace mint
{
    class Program
    {
        static void Main(string[] args)
        {
            dynamic obj = "a";
            obj.ToString();

            Debug.Assert(Marshal.SizeOf<Nil>()    <= sizeof(bool));
            Debug.Assert(Marshal.SizeOf<True>()   <= sizeof(bool));
            Debug.Assert(Marshal.SizeOf<False>()  <= sizeof(bool));
            Debug.Assert(Marshal.SizeOf<Fixnum>() <= sizeof(long));
            Debug.Assert(Marshal.SizeOf<Symbol>() <= IntPtr.Size);
            
            test.InvokeDynamicMethods.Test();
        }
        
    }
}
