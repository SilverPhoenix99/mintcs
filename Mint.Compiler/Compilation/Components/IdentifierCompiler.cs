using System;
using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class IdentifierCompiler : CompilerComponentBase
    {
        public IdentifierCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var closure = Compiler.CurrentScope.Closure;
            var name = new Symbol(Node.Value.Value);

            if(closure.IsDefined(name))
            {
                return closure.Variable(name);
            }

            throw new NotImplementedException("variable not found. methods not implemented.");
        }
    }
}