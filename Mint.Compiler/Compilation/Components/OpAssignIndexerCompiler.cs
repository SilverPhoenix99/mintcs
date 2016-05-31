using Mint.Binding.Arguments;
using Mint.Compilation.Components.Operators;
using Mint.Parse;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class OpAssignIndexerCompiler : OpAssignCompiler
    {
        private ParameterExpression instance;
        private ParameterExpression[] argumentVars;
        private InvocationArgument[] invocationArguments;

        private Ast<Token> ArgumentsNode => LeftNode[1];

        private int ArgumentCount => ArgumentsNode.List.Count;

        public override Expression Getter =>
            CompilerUtils.Call(instance, GetterMethodName, Visibility, invocationArguments);

        private Symbol GetterMethodName => Symbol.AREF;

        private Symbol SetterMethodName => Symbol.ASET;

        public OpAssignIndexerCompiler(Compiler compiler, OpAssignOperator operatorCompiler)
            : base(compiler, operatorCompiler)
        {
            instance = Variable(typeof(iObject), "instance");
        }

        public override void Shift()
        {
            Push(LeftNode[0]);
            PushArguments();
            Push(RightNode);
        }

        private void PushArguments()
        {
            foreach(var argument in ArgumentsNode)
            {
                Push(argument);
            }
        }

        public override Expression Reduce()
        {
            var left = Pop();
            var arguments = PopArguments();
            Right = Pop();

            argumentVars = CreateArgumentVariables();

            var argumentKinds = ArgumentsNode.Select(node => CompilerUtils.GetArgumentKind(node.Value.Type));

            invocationArguments =
                argumentKinds.Zip(argumentVars, (kind, arg) => new InvocationArgument(kind, arg)).ToArray();

            var argumentVarsAssignment = ArgumentCount == 0
                ? (Expression) Empty()
                : Block(argumentVars.Zip(arguments, (variable, argument) => Assign(variable, argument)));

            return Block(
                typeof(iObject),
                new[] { instance }.Concat(argumentVars),
                Assign(instance, left),
                argumentVarsAssignment,
                base.Reduce()
            );
        }

        private Expression[] PopArguments()
        {
            return Enumerable.Range(0, ArgumentCount).Select(_ => Pop()).ToArray();
        }

        private ParameterExpression[] CreateArgumentVariables()
        {
            return Enumerable.Range(0, ArgumentCount).Select(i => Variable(typeof(iObject), "arg" + i)).ToArray();
        }

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
