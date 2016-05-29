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
            var closure = Compiler.CurrentScope.Closure;
            var name = new Symbol(Identifier);

            if(closure.IsDefined(name))
            {
                return closure.Variable(name);
            }

            throw new NotImplementedException("variable not found. methods not implemented.");
        }
    }
}