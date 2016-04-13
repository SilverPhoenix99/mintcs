﻿using Mint;
using Mint.MethodBinding;
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
            
            //try
            //{
            //    TestCallSite();
            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine(e);
            //}
        }

        static void TestCallSite()
        {
            var site = new CallSite(new Symbol("test"), new[] { ParameterKind.Req }, new PolymorphicSiteBinder());
            var result = site.Call(new Fixnum(1), new Fixnum(42));
            Console.WriteLine(result);
        }

        /*
        static void OtherTests()
        {
            //TestGems.Test();
            //TestSymbolGC.Test();
            //InvokeDynamicMethods.Test();
            //TestRoslyn.TestLambdaCompilation();
            //TestExtensionReflection.MainTest();

            //if(InVisualStudio)
            //{
            //    TestInterpreter.Test("<<A", "blah", "A");
            //}
            //else
            //{
            //    TestInterpreter.Test(args);
            //}

            //if(InVisualStudio)
            //{
            //    TestCompiler.Test(":a?");
            //
            //    Console.WriteLine("------------------------------------------------------");
            //    Console.WriteLine();
            //
            //    TestCompiler.Test(":\"a#{:c;'b'}\"");
            //}
            //else
            //{
            //    TestCompiler.Test(args);
            //}

            //var fragment = File.ReadAllText(@"C:\Programming\Ruby\ruby22\lib\ruby\gems\2.2.0\gems\parser-2.3.0.1\lib\parser\lexer.rb");
            //var tokens = new Lexer(fragment).ToArray();

            //var ast = Parser.Parse(fragment);
            //AstPrinter<Token>.Print(ast, indent_size: 4);
        }
        */
    }
}
