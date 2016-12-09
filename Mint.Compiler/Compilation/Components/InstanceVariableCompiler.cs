using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class InstanceVariableCompiler : CompilerComponentBase
    {
        public InstanceVariableCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var instance = Compiler.CurrentScope.Instance;
            var variableName = Constant(new Symbol(Node.Value.Value));
            return Object.Expressions.InstanceVariableGet(instance, variableName);
        }
    }
}
