using System.Globalization;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class FloatCompiler : CompilerComponentBase
    {
        public FloatCompiler(Compiler compiler) : base(compiler)
        { }
        
        public override Expression Reduce()
        {
            var token = Node.Value;
            var str = token.Value.Replace("_", "");
            var val = System.Convert.ToDouble(str, CultureInfo.InvariantCulture);
            return Constant(new Float(val), typeof(iObject));
        }
    }
}