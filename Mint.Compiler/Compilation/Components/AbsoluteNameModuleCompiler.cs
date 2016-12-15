using System.Linq.Expressions;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class AbsoluteNameModuleCompiler : SimpleNameModuleCompiler
    {
        protected override Ast<Token> NameNode => Node[0][0];

        protected override Expression Container => Constant(Class.OBJECT);

        public AbsoluteNameModuleCompiler(Compiler compiler) : base(compiler)
        { }
    }
}