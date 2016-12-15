using Mint.Parse;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class AbsoluteResolutionCompiler : CompilerComponentBase
    {
        private Ast<Token> OperandNode => Node[0];

        private Symbol ConstantName => new Symbol(OperandNode.Value.Value);

        public AbsoluteResolutionCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile() =>
            Module.Expressions.GetConstant(Constant(Class.OBJECT), Constant(ConstantName));
    }
}
