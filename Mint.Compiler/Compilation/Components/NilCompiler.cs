﻿using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class NilCompiler : CompilerComponentBase
    {
        public NilCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce() => NilClass.Expressions.Instance;
    }
}
