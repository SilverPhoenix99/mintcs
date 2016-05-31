using Mint.Binding;
using Mint.Compilation.Components.Operators;
using Mint.Parse;
using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
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
}
