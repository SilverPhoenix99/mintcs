using mint.Compiler;
using mint.types;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

using Ex = System.Linq.Expressions.Expression;

namespace mint
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Assert(Marshal.SizeOf(typeof(Nil))    <= sizeof(bool));
            Debug.Assert(Marshal.SizeOf(typeof(True))   <= sizeof(bool));
            Debug.Assert(Marshal.SizeOf(typeof(False))  <= sizeof(bool));
            Debug.Assert(Marshal.SizeOf(typeof(Fixnum)) <= sizeof(long));
            Debug.Assert(Marshal.SizeOf(typeof(Symbol)) <= IntPtr.Size);

            Token[] tokens = new Lexer("def f").ToArray();
            
            test.InvokeDynamicMethods.Test();
        }
        
    }
}
