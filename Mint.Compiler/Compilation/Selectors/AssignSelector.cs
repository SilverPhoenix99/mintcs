using System;
using Mint.Compilation.Components;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Selectors
{
    internal class AssignSelector : ComponentSelectorBase
    {
        private CompilerComponent assignVariable;
        private CompilerComponent assignProperty;
        private CompilerComponent assignIndexer;

        private CompilerComponent AssignVariable =>
            assignVariable ?? (assignVariable = new AssignVariableCompiler(Compiler));

        private CompilerComponent AssignProperty =>
            assignProperty ?? (assignProperty = new AssignPropertyCompiler(Compiler));

        private CompilerComponent AssignIndexer =>
            assignIndexer ?? (assignIndexer = new AssignIndexerCompiler(Compiler));

        public AssignSelector(Compiler compiler) : base(compiler)
        { }

        public override CompilerComponent Select()
        {
            switch(Node[0].Value.Type)
            {
                case tIDENTIFIER:
                    return AssignVariable;

                case kDOT:
                    return AssignProperty;

                case kLBRACK2:
                    return AssignIndexer;

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
