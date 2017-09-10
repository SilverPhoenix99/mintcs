using Mint.MethodBinding.Arguments;
using Mint.Compilation.Components.Operators;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class AssignPropertyCompiler : AssignCompiler
    {
        private readonly ParameterExpression instance;

        public override Expression Getter => CompilerUtils.Call(instance, GetterMethodName, Visibility);

        private string MethodName => LeftNode[1].Token.Text;

        private Symbol GetterMethodName => new Symbol(MethodName);

        private Symbol SetterMethodName => new Symbol(MethodName + "=");

        public AssignPropertyCompiler(Compiler compiler, AssignOperator operatorCompiler)
            : base(compiler, operatorCompiler)
        {
            instance = Variable(typeof(iObject), "instance");
        }

        public override Expression Compile()
        {
            var left = LeftNode[0].Accept(Compiler);
            Right = RightNode.Accept(Compiler);

            return Block(
                typeof(iObject),
                new[] { instance },
                Assign(instance, left),
                base.Compile()
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
