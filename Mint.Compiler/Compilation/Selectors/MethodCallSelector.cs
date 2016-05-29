using Mint.Compilation.Components;
using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Selectors
{
    internal class MethodCallSelector : ComponentSelectorBase
    {
        private CompilerComponent publicMethodCall;
        private CompilerComponent privateMethodCall;
        private CompilerComponent safeMethodCall;

        private Ast<Token> LeftNode => Node[0];

        private CompilerComponent PublicMethodCall =>
            publicMethodCall ?? (publicMethodCall = new PublicMethodCallCompiler(Compiler));

        private CompilerComponent PrivateMethodCall =>
            privateMethodCall ?? (privateMethodCall = new PrivateMethodCallCompiler(Compiler));

        private CompilerComponent SafeMethodCall =>
            safeMethodCall ?? (safeMethodCall = new SafeMethodCallCompiler(Compiler));

        public MethodCallSelector(Compiler compiler) : base(compiler)
        { }

        public override CompilerComponent Select() =>
            IsSafeOperator() ? SafeMethodCall
            : HasLeftSide() ? PublicMethodCall
            : PrivateMethodCall;

        private bool IsSafeOperator() => Node.Value.Type == kANDDOT;

        private bool HasLeftSide() => !LeftNode.IsList || LeftNode.List.Count != 0;
    }
}
