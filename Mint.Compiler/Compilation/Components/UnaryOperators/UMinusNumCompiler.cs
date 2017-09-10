using System;
using System.Linq.Expressions;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class UMinusNumCompiler : CompilerComponentBase
    {
        private SyntaxNode Value => Node[0];

        public UMinusNumCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var constant = (ConstantExpression) Value.Accept(Compiler);
            var number = constant.Value;

            if(number is Fixnum)
            {
                number = new Fixnum(-(long) (Fixnum) number);
            }
            else if(number is Float)
            {
                number = new Float(-(double) (Float) number);
            }
            else if(number is Complex)
            {
                throw new NotImplementedException();
                //number = ((Complex) number).Conjugate();
            }
            else
            {
                throw new NotImplementedException();
                //number = -(Rational) number;
            }

            return Constant(number, typeof(iObject));
        }
    }
}