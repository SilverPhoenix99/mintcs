using Mint.MethodBinding;
using System;
using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class IdentifierCompiler : CompilerComponentBase
    {
        private string Identifier => Node.Value.Value;

        public IdentifierCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var name = new Symbol(Identifier);
            var variable = Compiler.CurrentScope.FindVariable(name);

            if(variable != null)
            {
                return variable.ValueExpression();
            }

            var instance = Compiler.CurrentScope.Instance;
            var arguments = System.Array.Empty<InvocationArgument>();
            return CompilerUtils.Call(instance, name, Visibility.Private, arguments);
        }
    }
}