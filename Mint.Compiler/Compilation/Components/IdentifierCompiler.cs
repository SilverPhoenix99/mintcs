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

            if(scope.Closure.IsDefined(name))
            {
                return LocalVariable.Expressions.Value(scope.Closure.GetLocal(name));
            }

            throw new NotImplementedException("variable not found. methods not implemented.");
        }
    }
}