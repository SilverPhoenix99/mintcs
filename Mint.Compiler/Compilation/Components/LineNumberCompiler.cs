using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class LineNumberCompiler : CompilerComponentBase
    {
        public LineNumberCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce() =>
            Expression.Constant(new Fixnum(Node.Value.Location.StartLine), typeof(iObject));
    }
}