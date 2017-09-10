using Mint.Parse;
using System;
using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class RelativeResolutionCompiler : CompilerComponentBase
    {
        private SyntaxNode LeftNode => Node[0];

        private SyntaxNode RightNode => Node[1];

        private Symbol ConstantName => new Symbol(RightNode.Token.Text);

        public RelativeResolutionCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var name = ConstantName;

            iObject ResolutionLambda(iObject obj)
            {
                if(obj is Module module)
                {
                    return module.GetConstant(name);
                }

                throw new TypeError(obj.Inspect() + " is not a class/module");
            }

            var left = LeftNode.Accept(Compiler);
            return Expression.Invoke(Expression.Constant((Func<iObject, iObject>) ResolutionLambda), left);
        }
    }
}
