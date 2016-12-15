using System;
using Mint.Compilation.Components;
using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Selectors
{
    internal class ClassSelector : ComponentSelectorBase
    {
        private ClassCompiler simpleName;
        private ClassCompiler relativeName;
        private ClassCompiler absoluteName;
        private ClassCompiler singleton;

        private Ast<Token> Name => Node[0];

        private TokenType Type => Name.Value.Type;

        private ClassCompiler SimpleName => simpleName ?? (simpleName = new SimpleNameClassCompiler(Compiler));

        private ClassCompiler AbsoluteName =>
            absoluteName ?? (absoluteName = new AbsoluteNameClassCompiler(Compiler));

        private ClassCompiler RelativeName =>
            relativeName ?? (relativeName = new RelativeNameClassCompiler(Compiler));

        private ClassCompiler Singleton => singleton ?? (singleton = new SingletonClassCompiler(Compiler));

        public ClassSelector(Compiler compiler) : base(compiler)
        { }

        public override CompilerComponent Select()
        {
            switch(Type)
            {
                case kCOLON2: return RelativeName;
                case kCOLON3: return AbsoluteName;
                case kLSHIFT: return Singleton;
                case tCONSTANT: return SimpleName;
                default:
                    throw new NotImplementedException($"Class compiler for type {Type}");
            }
        }
    }
}
