using mint.Compiler;
using mint.test;
using mint.types;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

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

            //Token[] tokens = new Lexer("def []=; end").ToArray();

            Ast<Token> ast = Parser.Parse("def []=; end");
            AstPrinter<Token>.Print(ast, indent_size: 4);
            
            //test.InvokeDynamicMethods.Test();
        }
        
    }
}
