using System;
using System.Linq.Expressions;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class AssignCompiler : CompilerComponentBase
    {
        private readonly CompilerComponent assignVariable;
        private readonly CompilerComponent assignProperty;
        private readonly CompilerComponent assignIndexer;

        public AssignCompiler(Compiler compiler) : base(compiler)
        {
            assignVariable = new AssignVariableCompiler(compiler);
            assignProperty = new AssignPropertyCompiler(compiler);
            assignIndexer = new AssignIndexerCompiler(compiler);
        }

        public override void Shift()
        {
            var component = GetComponentOrThrow();
            component.Shift();
        }

        public override Expression Reduce()
        {
            var component = GetComponentOrThrow();
            return component.Reduce();
        }

        private CompilerComponent GetComponentOrThrow()
        {
            switch (Node[0].Value.Type)
            {
                case tIDENTIFIER:
                    return assignVariable;

                case kDOT:
                    return assignProperty;

                case kLBRACK2:
                    return assignIndexer;

                case kSELF:
                {
                    var line = Node[0].Value.Location.Item1;
                    throw new SyntaxError(Compiler.Filename, line, "Can't change the value of self");
                }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}