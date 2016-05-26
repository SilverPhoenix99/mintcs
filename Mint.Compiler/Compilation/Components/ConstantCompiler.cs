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
                case kNIL:   return Compiler.NIL;
                case kFALSE: return Compiler.FALSE;
                case kTRUE:  return Compiler.TRUE;
                default:     return null;
            }
        }
    }
}