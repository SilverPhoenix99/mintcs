﻿using System.Linq.Expressions;

namespace Mint.Compilation.Components
{
    internal class PosfixWhileCompiler : WhileCompiler
    {
        public PosfixWhileCompiler(Compiler compiler) : base(compiler)
        { }

        protected override Expression Compile(Expression condition, Expression body)
        {
            var scope = Compiler.CurrentScope;

            /*
             * redo:
             *     body;
             * next:
             *     if(cond)
             *     {
             *         goto redo;
             *     }
             * break: nil;
             */

             throw new System.NotImplementedException();

            /*return Block(
                typeof(iObject),
                Label(scope.RedoLabel),
                body,
                Label(scope.NextLabel),
                IfThen(condition, Goto(scope.RedoLabel)),
                Label(scope.BreakLabel, NilClass.Expressions.Instance)
            );*/
        }
    }
}
