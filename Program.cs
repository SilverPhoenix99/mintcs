using mint.Compiler;
using mint.test;
using mint.types;
using System;
using System.Diagnostics;
using System.IO;
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


            var fragment = @"a = b?(:c)";
            
            var tokens = new Lexer(fragment).ToArray();
            
            var ast2 = Parser.Parse(fragment);
            var doc2 = AstXmlSerializer.ToXml(ast2);
            //Console.WriteLine(doc2.ToString());

            int count = 0;
            foreach(var file_name in Directory.EnumerateFiles(@"C:\Programming\Ruby\ruby22\lib\ruby\gems\2.2.0\gems", "*.rb", SearchOption.AllDirectories))
            {
                var file_text = File.ReadAllText(file_name);
                var rel_path = file_name.Substring(52);

                // not the best option:
                if(rel_path == @"actionmailer-4.2.5\lib\rails\generators\mailer\templates\mailer.rb")
                {
                    // it's not a ruby file
                    continue;
                }

                var ast = Parser.Parse(file_text);
                var doc = AstXmlSerializer.ToXml(ast);
                //Console.WriteLine(doc.ToString());
                ++count;
                Console.WriteLine($"Parsed {count} files: {rel_path}");
            }

            //var file_text = File.ReadAllText(@"C:\Programming\Ruby\ruby22\lib\ruby\gems\2.2.0\gems\pigment-0.2.3\lib\pigment.rb");

            //var tokens = new Lexer(file_text).ToArray();

            //var tokens = new Lexer("\"Color(r =#{r}#{\", [h=#{h}, s=#{s}]\" if @hsl})\"").ToArray();

            //var ast = Parser.Parse(file_text);

            //var doc = AstXmlSerializer.ToXml(ast);
            //Console.WriteLine(doc.ToString());

            //AstPrinter<Token>.Print(ast, indent_size: 4);

            //test.InvokeDynamicMethods.Test();
        }
        
    }
}
