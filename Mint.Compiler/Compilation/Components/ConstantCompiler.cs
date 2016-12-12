using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class ConstantCompiler : CompilerComponentBase
    {
        public ConstantCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var module = Compiler.CurrentScope.Module;
            var constantName = Constant(new Symbol(Node.Value.Value));
            return Module.Expressions.GetConstant(module, constantName);
        }
    }
}
