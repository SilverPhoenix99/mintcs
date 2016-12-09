using System.Linq.Expressions;
using Mint.Compilation.Components.Operators;

namespace Mint.Compilation.Components
{
    internal abstract class AssignVariableCompiler : AssignCompiler
    {
        private Expression getter;

        protected Symbol VariableName => new Symbol(LeftNode.Value.Value);

        public override Expression Getter => getter ?? (getter = CreateGetter());

        protected AssignVariableCompiler(Compiler compiler, AssignOperator operatorCompiler)
            : base(compiler, operatorCompiler)
        { }

        public override Expression Compile()
        {
            Right = RightNode.Accept(Compiler);
            return base.Compile();
        }

        protected abstract Expression CreateGetter();
    }
}
