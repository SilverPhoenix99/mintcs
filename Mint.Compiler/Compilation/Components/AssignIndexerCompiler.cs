using Mint.MethodBinding.Arguments;
using Mint.Compilation.Components.Operators;
using Mint.Parse;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class AssignIndexerCompiler : AssignCompiler
    {
        private readonly ParameterExpression instance;
        private InvocationArgument[] invocationArguments;

        private Ast<Token> ArgumentsNode => LeftNode[1];

        private int ArgumentCount => ArgumentsNode.List.Count;

        public override Expression Getter =>
            CompilerUtils.Call(instance, GetterMethodName, Visibility, invocationArguments);

        private Symbol GetterMethodName => Symbol.AREF;

        private Symbol SetterMethodName => Symbol.ASET;

        public AssignIndexerCompiler(Compiler compiler, AssignOperator operatorCompiler)
            : base(compiler, operatorCompiler)
        {
            instance = Variable(typeof(iObject), "instance");
        }

        public override Expression Compile()
        {
            var left = LeftNode[0].Accept(Compiler);
            var arguments = CompileArguments();
            Right = RightNode.Accept(Compiler);

            var argumentVars = CreateArgumentVariables();

            var argumentKinds = ArgumentsNode.Select(node => node.Value.Type.GetArgumentKind());

            invocationArguments =
                argumentKinds.Zip(argumentVars, (kind, arg) => new InvocationArgument(kind, arg)).ToArray();

            var argumentVarsAssignment = ArgumentCount == 0
                ? (Expression) Empty()
                : Block(argumentVars.Zip(arguments, Assign));

            return Block(
                typeof(iObject),
                new[] { instance }.Concat(argumentVars),
                Assign(instance, left),
                argumentVarsAssignment,
                base.Compile()
            );
        }

        private Expression[] CompileArguments() => ArgumentsNode.Select(_ => _.Accept(Compiler)).ToArray();

        private ParameterExpression[] CreateArgumentVariables() =>
            Enumerable.Range(0, ArgumentCount).Select(i => Variable(typeof(iObject), "arg" + i)).ToArray();


        public override Expression Setter(Expression rightHandSide)
        {
            var rightVar = Variable(typeof(iObject), "right");
            var rightArgument = new InvocationArgument(ArgumentKind.Simple, rightVar);
            var setterArguments = invocationArguments.Concat(new[]{ rightArgument }).ToArray();

            return Block(
                typeof(iObject),
                new[] { rightVar },
                Assign(rightVar, rightHandSide),
                CompilerUtils.Call(instance, SetterMethodName, Visibility, setterArguments),
                rightVar
            );
        }
    }
}
