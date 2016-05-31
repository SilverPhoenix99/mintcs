using System.Linq.Expressions;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class ConstantCompiler : CompilerComponentBase
    {
        public ConstantCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            switch(Node.Value.Type)
            {
                case kNIL:   return CompilerUtils.NIL;
                case kFALSE: return CompilerUtils.FALSE;
                case kTRUE:  return CompilerUtils.TRUE;
                default:     return null;
            }
        }
    }
}