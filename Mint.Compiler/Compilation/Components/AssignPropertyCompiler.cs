using Mint.Binding.Arguments;
using Mint.Compilation.Components.Operators;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class AssignPropertyCompiler : AssignCompiler
    {
        private readonly ParameterExpression instance;

        public override Expression Getter => CompilerUtils.Call(instance, GetterMethodName, Visibility);

        private string MethodName => LeftNode[1].Value.Value;

        private Symbol GetterMethodName => new Symbol(MethodName);

        private Symbol SetterMethodName => new Symbol(MethodName + "=");

        public AssignPropertyCompiler(Compiler compiler, AssignOperator operatorCompiler)
            : base(compiler, operatorCompiler)
        {
            instance = Variable(typeof(iObject), "instance");
        }

        public override void Shift()
        {
            Push(LeftNode[0]);
            Push(RightNode);
        }

        public override Expression Reduce()
        {
            var left = Pop();
            Right = Pop();

            return Block(
                typeof(iObject),
                new[] { instance },
                Assign(instance, left),
                base.Reduce()
            );
        }

        public override Expression Setter(Expression rightHandSide)
        {
            var rightVar = Variable(typeof(iObject), "right");
            var rightArgument = new InvocationArgument(ArgumentKind.Simple, rightVar);

            return Block(
                typeof(iObject),
                new[] { rightVar },
                Assign(rightVar, rightHandSide),
                CompilerUtils.Call(instance, SetterMethodName, Visibility, rightArgument),
                rightVar
            );
        }
    }
}
