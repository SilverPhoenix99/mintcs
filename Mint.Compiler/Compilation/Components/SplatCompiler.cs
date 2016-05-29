using System;
using System.Linq.Expressions;
using Mint.Binding;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class SplatCompiler : CompilerComponentBase
    {
        protected virtual Symbol MethodName => Symbol.TO_ARY;
        protected virtual Type ElementType => typeof(Array);

        public SplatCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var operand = Pop();
            var call = CompilerUtils.Call(operand, MethodName, Visibility.Private);

            return Condition(
                TypeIs(operand, ElementType),
                operand,
                call,
                typeof(iObject)
            );
        }
    }
}
