using Mint.Parse;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class IntegerCompiler : BaseCompilerComponent
    {
        private static readonly Regex CLEAN_INTEGER = new Regex(@"[_BODX]", RegexOptions.Compiled);

        public IntegerCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile(Ast<Token> ast)
        {
            var tok = ast.Value;
            var str = CLEAN_INTEGER.Replace(tok.Value.ToUpper(), "");
            var numBase = (int) tok.Properties["num_base"];
            var val = System.Convert.ToInt64(str, numBase);
            return Constant(new Fixnum(val), typeof(iObject));
        }
    }
}