using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint
{
    public class CompiledMethod : Method
    {
        public CompiledMethod(Symbol name, Module owner, MethodInfo info) : base(name, owner)
        {
            MethodInfo = info;
        }

        public MethodInfo MethodInfo { get; }

        public override Expression Bind(Expression target, params Expression[] args)
        {
            if(MethodInfo.IsStatic)
            {
                return Call(MethodInfo, new[] { target }.Concat(args));
            }

            return Call(target, MethodInfo, args);
        }
    }
}
