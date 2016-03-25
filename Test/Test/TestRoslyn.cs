//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace Mint.Test
{
    internal static class TestRoslyn
    {
        public static void TestLambdaCompilation()
        {
            /*
            var code = "((System.Func<dynamic, dynamic>) (x => x + 1))";
            var tree = CSharpSyntaxTree.ParseText(code, CSharpParseOptions.Default.WithKind(SourceCodeKind.Script));

            var compileOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithScriptClassName("X");

            var compilation = CSharpCompilation.CreateScriptCompilation(
                "x",
                tree,
                new MetadataReference[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(CSharpArgumentInfo).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(IDynamicMetaObjectProvider).Assembly.Location),
                },
                compileOptions,
                null,
                typeof(Func<dynamic, dynamic>));

            var stream = new MemoryStream();
            var emitResult = compilation.Emit(stream);
            stream.Position = 0;

            foreach(var msg in from d in emitResult.Diagnostics
                               where d.IsWarningAsError || d.Severity >= DiagnosticSeverity.Error
                               select d.GetMessage())
            {
                Console.WriteLine(msg);
                Debug.WriteLine(msg);
            }
            */
        }
    }
}
