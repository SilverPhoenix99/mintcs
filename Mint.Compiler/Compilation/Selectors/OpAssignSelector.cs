using System.Linq.Expressions;
using Mint.Binding;
using Mint.Binding.Arguments;
using Mint.Compilation.Components;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Selectors
{
    internal class OpAssignSelector : ComponentSelectorBase
    {
        public OpAssignSelector(Compiler compiler) : base(compiler)
        { }

        public override CompilerComponent Select()
        {
            throw new System.NotImplementedException();
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
            return Condition(
                Call(CompilerUtils.IS_NIL, component.Getter),
                component.Setter(component.Right),
                component.Getter
            );
        }
    }

    internal class AndAssignOperator : OpAssignOperator
    {
        public Expression Reduce(OpAssignCompiler component)
        {
            return Condition(
                Call(CompilerUtils.IS_NIL, component.Getter),
                component.Getter,
                component.Setter(component.Right)
            );
        }
    }

    internal class GenericOpAssignOperator : OpAssignOperator
    {
        public Expression Reduce(OpAssignCompiler component)
        {
            var instance = component.Getter;
            var visibility = component.GetVisibility();
            var argument = new InvocationArgument(ArgumentKind.Simple, component.Right);
            var call = CompilerUtils.Call(instance, component.Operator, visibility, argument);
            return component.Setter(call);
        }
    }

    //-------------------------------------------------------------

    internal abstract class OpAssignCompiler : CompilerComponentBase
    {
        protected Ast<Token> LeftNode => Node[0];
        public Symbol Operator => new Symbol(Node.Value.Value);
        protected OpAssignOperator OperatorCompiler { get; }

        public abstract Expression Getter { get; }
        public abstract Expression Right { get; }

        protected OpAssignCompiler(Compiler compiler, OpAssignOperator operatorCompiler) : base(compiler)
        {
            OperatorCompiler = operatorCompiler;
        }

        public abstract Expression Setter(Expression rightHandSide);

        public Visibility GetVisibility() => CompilerUtils.GetVisibility(LeftNode);
    }

    internal class VariableOpAssignCompiler : OpAssignCompiler
    {
        public override Expression Getter { get; }

        public override Expression Right { get; }

        public VariableOpAssignCompiler(Compiler compiler, OpAssignOperator operatorCompiler)
            : base(compiler, operatorCompiler)
        { }

        public override Expression Reduce()
        {
            throw new System.NotImplementedException();
        }

        public override Expression Setter(Expression rightHandSide)
        {
            return Assign(Getter, rightHandSide);
        }
    }

    internal class PropertyOpAssignCompiler : OpAssignCompiler
    {
        public override Expression Getter { get; }

        public override Expression Right { get; }

        public PropertyOpAssignCompiler(Compiler compiler, OpAssignOperator operatorCompiler)
            : base(compiler, operatorCompiler)
        { }

        public override Expression Reduce()
        {
            throw new System.NotImplementedException();
        }

        public override Expression Setter(Expression rightHandSide)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class IndexerOpAssignCompiler : OpAssignCompiler
    {
        public override Expression Getter { get; }

        public override Expression Right { get; }

        public IndexerOpAssignCompiler(Compiler compiler, OpAssignOperator operatorCompiler)
            : base(compiler, operatorCompiler)
        { }

        public override Expression Reduce()
        {
            throw new System.NotImplementedException();
        }

        public override Expression Setter(Expression rightHandSide)
        {
            throw new System.NotImplementedException();
        }
    }
}
