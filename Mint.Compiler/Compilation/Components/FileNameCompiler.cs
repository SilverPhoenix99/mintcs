using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class FileNameCompiler : CompilerComponentBase
    {
        public FileNameCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce() =>
            Constant(new String(Compiler.Filename), typeof(iObject));
    }
}