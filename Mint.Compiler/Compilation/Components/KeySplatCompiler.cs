using System;

namespace Mint.Compilation.Components
{
    internal class KeySplatCompiler : SplatCompiler
    {
        protected override Symbol MethodName => Symbol.TO_HASH;
        protected override Type ElementType => typeof(Hash);

        public KeySplatCompiler(Compiler compiler) : base(compiler)
        { }
    }
}
