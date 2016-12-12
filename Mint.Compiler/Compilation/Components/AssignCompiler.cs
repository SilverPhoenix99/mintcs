using Mint.MethodBinding;
using Mint.Compilation.Components.Operators;
using Mint.Parse;
using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal abstract class AssignCompiler : CompilerComponentBase
    {
        private readonly AssignOperator operatorCompiler;

        protected Ast<Token> LeftNode => Node[0];
        protected Ast<Token> RightNode => Node[1];
        public Symbol Operator => new Symbol(Node.Value.Value);

        public abstract Expression Getter { get; }
        public Expression Right { get; protected set; }
        public Visibility Visibility => LeftNode.GetVisibility();

        protected AssignCompiler(Compiler compiler, AssignOperator operatorCompiler) : base(compiler)
        {
            this.operatorCompiler = operatorCompiler;
        }

        public override Expression Compile() => operatorCompiler.Compile(this);

        public abstract Expression Setter(Expression rightHandSide);
    }
}
