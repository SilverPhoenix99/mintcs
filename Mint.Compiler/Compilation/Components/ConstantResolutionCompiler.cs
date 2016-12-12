using Mint.Parse;
using System;
using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class ConstantResolutionCompiler : CompilerComponentBase
    {
        private Ast<Token> LeftNode => Node[0];

        private Ast<Token> RightNode => Node[1];

        public ConstantResolutionCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var name = new Symbol(RightNode.Value.Value);

            Func<iObject, iObject> resolutionLambda = (obj) =>
            {
                if(obj is Module)
                {
                    return ((Module) obj).GetConstant(name);
                }

                throw new TypeError(obj.Inspect() + " is not a class/module");
            };

            var left = LeftNode.Accept(Compiler);
            return Expression.Invoke(Expression.Constant(resolutionLambda), left);
        }
    }
}