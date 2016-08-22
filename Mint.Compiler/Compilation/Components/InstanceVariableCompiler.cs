using System;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class InstanceVariableCompiler : CompilerComponentBase
    {
        public InstanceVariableCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var instance = Compiler.CurrentScope.Closure.Self;
            var variableName = Constant(new Symbol(Node.Value.Value));
            return Call(instance, CompilerUtils.INSTANCE_VARIABLE_GET, variableName);
        }
    }
}
