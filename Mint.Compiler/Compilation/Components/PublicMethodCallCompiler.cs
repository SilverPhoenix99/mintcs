﻿using System.Linq.Expressions;
using Mint.Binding;
using Mint.Parse;

namespace Mint.Compilation.Components
{
    internal class PublicMethodCallCompiler : PrivateMethodCallCompiler
    {
        private Ast<Token> LeftNode => Node[0];

        public PublicMethodCallCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            Push(LeftNode);
            base.Shift();
        }

        protected override Expression GetLeftExpression() => Pop();

        protected override Visibility GetVisibility() => GetVisibility(LeftNode);
    }
}
