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

            if(scope.IsDefined(name))
            {
                return scope.Variable(name);
            }

            throw new NotImplementedException("variable not found. methods not implemented.");
        }
    }
}