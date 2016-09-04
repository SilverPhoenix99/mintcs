using Mint.MethodBinding;
using Mint.Parse;
using static Mint.Parse.TokenType;
using static Mint.MethodBinding.Visibility;

namespace Mint.Compilation
{
    internal static class AstExtensions
    {
        public static Visibility GetVisibility(this Ast<Token> left)
        {
            // TODO if protected in instance_eval, and lhs != self but same class => public

            return left.Value?.Type == kSELF ? Protected : Public;
        }
    }
}
