using Mint.Compilation.Components;
using Mint.Parse;

namespace Mint.Compilation.Selectors
{
    internal class MethodCallSelector : ComponentSelectorBase
    {
        private CompilerComponent publicMethodCall;
        private CompilerComponent privateMethodCall;

        private Ast<Token> LeftNode => Node[0];

        private CompilerComponent PublicMethodCall =>
            publicMethodCall ?? (publicMethodCall = new PublicMethodCallCompiler(Compiler));

        private CompilerComponent PrivateMethodCall =>
            privateMethodCall ?? (privateMethodCall = new PrivateMethodCallCompiler(Compiler));

        public MethodCallSelector(Compiler compiler) : base(compiler)
        { }

        public override CompilerComponent Select() => HasLeftSide() ? PublicMethodCall : PrivateMethodCall;

        private bool HasLeftSide() => !LeftNode.IsList || LeftNode.List.Count != 0;
    }
}
