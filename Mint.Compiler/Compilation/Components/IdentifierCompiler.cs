using Mint.MethodBinding.Methods;
using System;
using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class IdentifierCompiler : CompilerComponentBase
    {
        private string Identifier => Node.Value.Value;

        private Symbol VariableName => new Symbol(Identifier);

        public IdentifierCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var variable = Compiler.CurrentScope.FindVariable(VariableName);

            if(variable == null)
            {
                throw new NotImplementedException("variable not found. methods not implemented.");
            }

            return variable.ValueExpression();
        }
    }
}