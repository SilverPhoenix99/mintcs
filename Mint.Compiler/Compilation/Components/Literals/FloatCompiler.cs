using System;
using System.Globalization;
using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class FloatCompiler : CompilerComponentBase
    {
        public FloatCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var token = Node.Token;
            var str = token.Text.Replace("_", "");
            var val = Convert.ToDouble(str, CultureInfo.InvariantCulture);
            return Expression.Constant(new Float(val), typeof(iObject));
        }
    }
}