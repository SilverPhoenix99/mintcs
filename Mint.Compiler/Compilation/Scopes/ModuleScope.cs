using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Scopes
{
    public class ModuleScope : MethodScope
    {
        public override Expression Nesting => Module;

        public override Expression Instance => Module.Cast<iObject>();

        public override Expression Module { get; }

        protected ModuleScope(Compiler compiler, Expression module) : base(compiler)
        {
            Module = module;
        }

        public ModuleScope(Compiler compiler) : this(compiler, Expression.Variable(typeof(Module), "module"))
        { }

        public override Expression CompileBody(Expression body)
        {
            return Block(
                new[] { Module as ParameterExpression }.Concat(variables.Select(v => v.Value.Local)),
                body
            );
        }
    }
}
