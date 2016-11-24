using Mint.MethodBinding.Methods;
using System;
using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class IdentifierCompiler : CompilerComponentBase
    {
        private string Identifier => Node.Value.Value;

        public IdentifierCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var scope = Compiler.CurrentScope;
            var name = new Symbol(Identifier);

            if(scope.Variables.ContainsKey(name))
            {
                return LocalVariable.Expressions.Value(scope.Variables[name].Local);
            }

            throw new NotImplementedException("variable not found. methods not implemented.");
        }
    }
}