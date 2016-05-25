using Mint.Parse;
using System.Linq.Expressions;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class ConstantCompiler : BaseCompilerComponent
    {
        public ConstantCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile(Ast<Token> ast)
        {
            switch(ast.Value.Type)
            {
                case kFALSE: return Compiler.FALSE;
                case kNIL:   return Compiler.NIL;
                case kTRUE:  return Compiler.TRUE;
                default:     return null;
            }
        }
    }
}