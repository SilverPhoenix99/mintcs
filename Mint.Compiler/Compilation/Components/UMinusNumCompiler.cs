using System;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class UMinusNumCompiler : CompilerComponentBase
    {
        public UMinusNumCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift() => Push(Node[0]);

        public override Expression Reduce()
        {
            var number = (iObject) ((ConstantExpression) Pop()).Value;

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