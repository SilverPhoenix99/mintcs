using Mint.Parse;
using System;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class RelativeResolutionCompiler : CompilerComponentBase
    {
        private Ast<Token> LeftNode => Node[0];

        private Ast<Token> RightNode => Node[1];

        private Symbol ConstantName => new Symbol(RightNode.Value.Value);

        public RelativeResolutionCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var name = ConstantName;

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

    internal class AbsoluteResolutionCompiler : CompilerComponentBase
    {
        private Ast<Token> OperandNode => Node[0];

        private Symbol ConstantName => new Symbol(OperandNode.Value.Value);

        public AbsoluteResolutionCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile() =>
            Module.Expressions.GetConstant(Constant(Class.OBJECT), Constant(ConstantName));
    }
}