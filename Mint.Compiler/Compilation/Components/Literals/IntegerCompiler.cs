using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Mint.Compilation.Components
{
    internal class IntegerCompiler : CompilerComponentBase
    {
        private static readonly Regex CLEAN_INTEGER = new Regex(@"[_BODX]", RegexOptions.Compiled);

        public IntegerCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
        {
            var token = Node.Token;
            var str = CLEAN_INTEGER.Replace(token.Text.ToUpper(), "");
            var numBase = (int) token.Properties["num_base"];
            var val = Convert.ToInt64(str, numBase);
            return Expression.Constant(new Fixnum(val), typeof(iObject));
        }
    }
}