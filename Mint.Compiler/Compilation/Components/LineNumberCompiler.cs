using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class LineNumberCompiler : CompilerComponentBase
    {
        public LineNumberCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile() =>
            Expression.Constant(new Fixnum(Node.Token.Location.StartLine), typeof(iObject));
    }
}