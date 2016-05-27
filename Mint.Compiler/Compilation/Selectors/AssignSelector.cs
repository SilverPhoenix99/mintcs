using System;
using Mint.Compilation.Components;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Selectors
{
    public class AssignSelector : ComponentSelector
    {
        private readonly Compiler compiler;
        private CompilerComponent assignVariable;
        private CompilerComponent assignProperty;
        private CompilerComponent assignIndexer;

        private CompilerComponent AssignVariable =>
            assignVariable ?? (assignVariable = new AssignVariableCompiler(compiler));

        private CompilerComponent AssignProperty =>
            assignProperty ?? (assignProperty = new AssignPropertyCompiler(compiler));

        private CompilerComponent AssignIndexer =>
            assignIndexer ?? (assignIndexer = new AssignIndexerCompiler(compiler));

        public AssignSelector(Compiler compiler)
        {
            this.compiler = compiler;
        }

        public CompilerComponent Select()
        {
            switch(compiler.CurrentNode[0].Value.Type)
            {
                case tIDENTIFIER:
                    return AssignVariable;

                case kDOT:
                    return AssignProperty;

                case kLBRACK2:
                    return AssignIndexer;

                case kSELF:
                {
                    var line = compiler.CurrentNode[0].Value.Location.Item1;
                    throw new SyntaxError(compiler.Filename, line, "Can't change the value of self");
                }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
