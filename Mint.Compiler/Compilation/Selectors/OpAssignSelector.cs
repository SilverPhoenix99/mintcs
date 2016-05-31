using System.Linq;
using System.Linq.Expressions;
using Mint.Binding;
using Mint.Binding.Arguments;
using Mint.Compilation.Components;
using Mint.Parse;
using static System.Linq.Expressions.Expression;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Selectors
{
    internal class OpAssignSelector : ComponentSelectorBase
    {
        private const string OP_OP = "||";
        private const string AND_OP = "&&";

        private Ast<Token> LeftNode => Node[0];
        private Ast<Token> RightNode => Node[1];

        public OpAssignSelector(Compiler compiler) : base(compiler)
        { }

        public override CompilerComponent Select()
        {
            var operatorCompiler = CreateOperator();

            switch(LeftNode.Value.Type)
            {
                case tIDENTIFIER:
                    return new VariableOpAssignCompiler(Compiler, operatorCompiler);

                //case kDOT:
                //    return new PropertyOpAssignCompiler(Compiler, operatorCompiler);

                case kLBRACK2:
                    return new IndexerOpAssignCompiler(Compiler, operatorCompiler);
            }

            throw new System.NotImplementedException();
        }

        private OpAssignOperator CreateOperator()
        {
            return Node.Value.Value == OP_OP ? new OrAssignOperator()
                 : Node.Value.Value == AND_OP ? new AndAssignOperator()
                 : (OpAssignOperator) new GenericOpAssignOperator();
        }
    }

    //-------------------------------------------------------------

    internal interface OpAssignOperator
    {
        Expression Reduce(OpAssignCompiler component);
    }

    internal class OrAssignOperator : OpAssignOperator
    {
        public Expression Reduce(OpAssignCompiler component)
        {
            var getter = Variable(typeof(iObject), "getter");
            var setter = component.Setter(component.Right);

            return Block(
                typeof(iObject),
                new[] { getter },
                Assign(getter, component.Getter),
                Condition(
                    CompilerUtils.ToBool(getter),
                    TrueOption(getter, setter),
                    FalseOption(getter, setter)
                )
            );
        }

        protected virtual Expression TrueOption(Expression left, Expression right) => left;
        protected virtual Expression FalseOption(Expression left, Expression right) => right;
    }

    internal class AndAssignOperator : OrAssignOperator
    {
        protected override Expression TrueOption(Expression left, Expression right) => right;
        protected override Expression FalseOption(Expression left, Expression right) => left;
    }

    internal class GenericOpAssignOperator : OpAssignOperator
    {
        public Expression Reduce(OpAssignCompiler component)
        {
            var argument = new InvocationArgument(ArgumentKind.Simple, component.Right);
            var call = CompilerUtils.Call(component.Getter, component.Operator, component.Visibility, argument);
            return component.Setter(call);
        }
    }

    //-------------------------------------------------------------

    internal abstract class OpAssignCompiler : CompilerComponentBase
    {
        protected Ast<Token> LeftNode => Node[0];
        protected Ast<Token> RightNode => Node[1];
        public Symbol Operator => new Symbol(Node.Value.Value);
        protected OpAssignOperator OperatorCompiler { get; }

        public abstract Expression Getter { get; }
        public Expression Right { get; protected set; }
        public Visibility Visibility => CompilerUtils.GetVisibility(LeftNode);

        protected OpAssignCompiler(Compiler compiler, OpAssignOperator operatorCompiler) : base(compiler)
        {
            OperatorCompiler = operatorCompiler;
        }

        public override Expression Reduce() => OperatorCompiler.Reduce(this);

        public abstract Expression Setter(Expression rightHandSide);
    }

    internal class VariableOpAssignCompiler : OpAssignCompiler
    {
        private Expression getter;

        public override Expression Getter => getter;

        private string VariableName => Node[0].Value.Value;

        public VariableOpAssignCompiler(Compiler compiler, OpAssignOperator operatorCompiler)
            : base(compiler, operatorCompiler)
        { }

        public override void Shift()
        {
            Push(RightNode);
        }

        public override Expression Reduce()
        {
            Right = Pop();

            var varName = new Symbol(VariableName);
            getter = Compiler.CurrentScope.Closure.Variable(varName);

            return base.Reduce();
        }

        public override Expression Setter(Expression rightHandSide) => Assign(Getter, rightHandSide);
    }

    internal class PropertyOpAssignCompiler : OpAssignCompiler
    {
        private ParameterExpression instance;

        public override Expression Getter => CompilerUtils.Call(instance, GetterMethodName, Visibility);

        private string MethodName => LeftNode[1].Value.Value;

        private Symbol GetterMethodName => new Symbol(MethodName);

        private Symbol SetterMethodName => new Symbol(MethodName + "=");

        public PropertyOpAssignCompiler(Compiler compiler, OpAssignOperator operatorCompiler)
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

    internal class IndexerOpAssignCompiler : OpAssignCompiler
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

        public IndexerOpAssignCompiler(Compiler compiler, OpAssignOperator operatorCompiler)
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
