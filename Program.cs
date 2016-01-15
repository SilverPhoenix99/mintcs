using mint.Compiler;
using mint.test;
using mint.types;
using System;
using System.Diagnostics;
using System.IO;
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
            Ast<Token> ast = Parser.Parse(
/*@"

class X
    def []=(i, v)
        @a[i] = v
    end

    alias :set :[]=
end

                "*/

/*@"module Math
  TAU = 2 * PI
end"*/

@"%W'a b c#@d  '"
);
                

            //Ast<Token> ast = Parser.Parse(File.ReadAllText(
            //    @"D:\Users\Silver Phoenix\Projects\RubyMine\mathgl\lib\version.rb"));

            var doc = AstXmlSerializer.ToXml(ast);
            Console.WriteLine(doc.ToString());
            
            //AstPrinter<Token>.Print(ast, indent_size: 4);

            //test.InvokeDynamicMethods.Test();
        }
        
    }
}
