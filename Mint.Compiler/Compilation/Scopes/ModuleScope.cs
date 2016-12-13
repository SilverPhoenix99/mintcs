using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Scopes
{
    public class ModuleScope : MethodScope
    {
        public override Expression Nesting { get; }

        public override Expression Instance => Module.Cast<iObject>();

        public override Expression Module { get; }

        public ModuleScope(Compiler compiler) : base(compiler)
        {
            Module = Expression.Variable(typeof(Module), "module");
            Nesting = Expression.Variable(typeof(IList<Module>), "nesting");
        }

        public override Expression CompileBody(Expression body)
        {
            return Block(
                new[] { Module as ParameterExpression, Nesting as ParameterExpression }.Concat(
                    variables.Select(v => v.Value.Local)
                ),
                body
            );
        }
    }
}
