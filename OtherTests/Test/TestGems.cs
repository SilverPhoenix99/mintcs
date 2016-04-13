using System;
using System.IO;
using Mint.Parse;

namespace Mint.Test
{
    internal static class TestGems
    {
        public static void Test()
        {
            var count = 0;
            foreach(var fileName in Directory.EnumerateFiles(@"C:\Programming\Ruby\ruby22\lib\ruby\gems\2.2.0\gems", "*.rb", SearchOption.AllDirectories))
            {
                var fileText = File.ReadAllText(fileName);
                var relPath = fileName.Substring(52);

                // not the best option:
                if(relPath == @"actionmailer-4.2.5\lib\rails\generators\mailer\templates\mailer.rb"
                || relPath == @"activejob-4.2.5\lib\rails\generators\job\templates\job.rb"
                || relPath == @"activerecord-4.2.5\lib\rails\generators\active_record\migration\templates\create_table_migration.rb"
                || relPath == @"activerecord-4.2.5\lib\rails\generators\active_record\migration\templates\migration.rb"
                || relPath == @"activerecord-4.2.5\lib\rails\generators\active_record\model\templates\model.rb"
                || relPath == @"activerecord-4.2.5\lib\rails\generators\active_record\model\templates\module.rb"
                || relPath.StartsWith(@"backports-3.6.7\spec\tags\")
                || relPath == @"erubis-2.7.0\lib\erubis\helpers\rails_form_helper.rb"
                || relPath == @"facets-3.0.0\lib\core\facets\enumerable\hashify.rb"
                || relPath.StartsWith(@"jbuilder-2.4.0\lib\generators\rails\templates\")
                || relPath.StartsWith(@"opal-0.8.1\spec\opal\")
                || relPath.StartsWith(@"opal-0.9.2\spec\opal\")
                || relPath == @"opal-rails-0.8.1\lib\rails\generators\opal\assets\templates\javascript.js.rb"
                || relPath.StartsWith(@"pik-0.2.8\") // not testing pik. wrong .rb extension in yaml files
                || relPath.StartsWith(@"railties-4.2.5\lib\rails\generators\")
                || relPath == @"rspec-0.9.4\lib\spec\matchers\be.rb"
                || relPath.StartsWith(@"rspec-rails-3.4.0\lib\generators\")
                || relPath.StartsWith(@"thor-0.19.1\spec\fixtures\doc\")
                || relPath.StartsWith(@"thor-0.19.1\spec\sandbox\doc\")
                || relPath == @"win-ffi-0.3.2\lib\win-ffi\functions\winmm.rb"
                //|| new Regex("^[a-u]").IsMatch(relPath)
                )
                {
                    // it's not a ruby file
                    continue;
                }

                var ast = Parser.Parse("(test gems)", fileText);
                //var doc = AstXmlSerializer.ToXml(ast);
                //Console.WriteLine(doc.ToString());
                ++count;
                Console.WriteLine($"Parsed {count} files: {relPath}");
            }
        }
    }
}
