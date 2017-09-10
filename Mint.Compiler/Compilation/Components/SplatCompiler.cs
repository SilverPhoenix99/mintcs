using System;
using System.Linq.Expressions;
using Mint.MethodBinding;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class SplatCompiler : CompilerComponentBase
    {
        protected virtual Symbol MethodName => Symbol.TO_ARY;

        protected virtual Type ElementType => typeof(Array);

        private SyntaxNode Operand => Node[0];

        public SplatCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var operand = Operand.Accept(Compiler);
            var convertCall = CompilerUtils.Call(operand, MethodName, Visibility.Private);

            return Condition(
                TypeIs(operand, ElementType),
                operand,
                convertCall,
                typeof(iObject)
            );
        }
    }
}
