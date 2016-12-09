using Mint.Parse;
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

        public override Expression Reduce()
        {
            var token = Node.Value;
            var str = CLEAN_INTEGER.Replace(token.Value.ToUpper(), "");
            var numBase = (int) token.Properties["num_base"];
            var val = Convert.ToInt64(str, numBase);
            return Expression.Constant(new Fixnum(val), typeof(iObject));
        }

        public override Expression Compile()
        {
            var token = Node.Value;
            var str = CLEAN_INTEGER.Replace(token.Value.ToUpper(), "");
            var numBase = (int) token.Properties["num_base"];
            var val = Convert.ToInt64(str, numBase);
            return Expression.Constant(new Fixnum(val), typeof(iObject));
        }
    }
}