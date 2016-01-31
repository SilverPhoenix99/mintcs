using Mint.Parser;
using Mint.Types;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Test
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

            //var fragment = File.ReadAllText(@"C:\Programming\Ruby\ruby22\lib\ruby\gems\2.2.0\gems\rspec-0.9.4\lib\spec\matchers\be.rb");
            var fragment = "f.x(:a, b, 'c')";

            var tokens = new Lexer(fragment).ToArray();
            var ast2 = Parser.Parse(fragment);
            var doc2 = AstXmlSerializer.ToXml(ast2);

            //Console.WriteLine(doc2.ToString());
            //AstPrinter<Token>.Print(ast2, indent_size: 4);

            int count = 0;
            foreach(var file_name in Directory.EnumerateFiles(@"C:\Programming\Ruby\ruby22\lib\ruby\gems\2.2.0\gems", "*.rb", SearchOption.AllDirectories))
            {
                var file_text = File.ReadAllText(file_name);
                var rel_path = file_name.Substring(52);

                // not the best option:
                if(rel_path == @"actionmailer-4.2.5\lib\rails\generators\mailer\templates\mailer.rb"
                || rel_path == @"activejob-4.2.5\lib\rails\generators\job\templates\job.rb"
                || rel_path == @"activerecord-4.2.5\lib\rails\generators\active_record\migration\templates\create_table_migration.rb"
                || rel_path == @"activerecord-4.2.5\lib\rails\generators\active_record\migration\templates\migration.rb"
                || rel_path == @"activerecord-4.2.5\lib\rails\generators\active_record\model\templates\model.rb"
                || rel_path == @"activerecord-4.2.5\lib\rails\generators\active_record\model\templates\module.rb"
                || rel_path.StartsWith(@"backports-3.6.7\spec\tags\")
                || rel_path == @"erubis-2.7.0\lib\erubis\helpers\rails_form_helper.rb"
                || rel_path == @"facets-3.0.0\lib\core\facets\enumerable\hashify.rb"
                || rel_path.StartsWith(@"jbuilder-2.4.0\lib\generators\rails\templates\")
                || rel_path == @"mail-2.6.3\lib\mail\parsers\ragel\ruby\machines\address_lists_machine.rb" // performance issue with gppg
                || rel_path.StartsWith(@"opal-0.8.1\spec\opal\")
                || rel_path.StartsWith(@"opal-0.9.2\spec\opal\")
                || rel_path == @"opal-rails-0.8.1\lib\rails\generators\opal\assets\templates\javascript.js.rb"
                || rel_path == @"parser-2.3.0.1\lib\parser\lexer.rb" // performance issue with gppg
                || rel_path == @"parser-2.3.0.1\lib\parser\ruby23.rb" // performance issue with gppg
                || rel_path == @"parser-2.3.0.1\lib\parser\ruby22.rb" // performance issue with gppg
                || rel_path == @"parser-2.3.0.1\lib\parser\ruby21.rb" // performance issue with gppg
                || rel_path == @"parser-2.3.0.1\lib\parser\ruby20.rb" // performance issue with gppg
                || rel_path == @"parser-2.3.0.1\lib\parser\macruby.rb" // performance issue with gppg
                || rel_path == @"parser-2.3.0.1\lib\parser\rubymotion.rb" // performance issue with gppg
                || rel_path == @"parser-2.3.0.1\lib\parser\ruby19.rb" // performance issue with gppg
                || rel_path == @"parser-2.3.0.1\lib\parser\ruby18.rb" // performance issue with gppg
                || rel_path.StartsWith(@"pik-0.2.8\") // not testing pik. wrong .rb extension in yaml files
                || rel_path.StartsWith(@"railties-4.2.5\lib\rails\generators\")
                || rel_path == @"rspec-0.9.4\lib\spec\matchers\be.rb"
                || rel_path.StartsWith(@"rspec-rails-3.4.0\lib\generators\")
                || rel_path.StartsWith(@"thor-0.19.1\spec\fixtures\doc\")
                || rel_path.StartsWith(@"thor-0.19.1\spec\sandbox\doc\")
                || rel_path == @"win-ffi-0.3.2\lib\win-ffi\functions\winmm.rb"
                //|| new Regex("^[a-u]").IsMatch(rel_path)
                )
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
            
            //test.InvokeDynamicMethods.Test();
        }
        
    }
}
