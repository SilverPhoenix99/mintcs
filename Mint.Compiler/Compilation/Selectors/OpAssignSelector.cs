using Mint.Parse;
using Mint.Compilation.Components;
using Mint.Compilation.Components.Operators;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Selectors
{
    internal class OpAssignSelector : ComponentSelectorBase
    {
        private const string OP_OP = "||";
        private const string AND_OP = "&&";

        private Ast<Token> LeftNode => Node[0];
        private Ast<Token> RightNode => Node[1];

        public OpAssignSelector(Compiler compiler) : base(compiler)
        { }

        public override CompilerComponent Select()
        {
            var operatorCompiler = CreateOperator();

            switch(LeftNode.Value.Type)
            {
                case tIDENTIFIER:
                    return new OpAssignVariableCompiler(Compiler, operatorCompiler);

                case kDOT:
                    return new OpAssignPropertyCompiler(Compiler, operatorCompiler);

                case kLBRACK2:
                    return new OpAssignIndexerCompiler(Compiler, operatorCompiler);
            }

            throw new System.NotImplementedException();
        }

        private OpAssignOperator CreateOperator()
        {
            return Node.Value.Value == OP_OP ? new OrAssignOperator()
                 : Node.Value.Value == AND_OP ? new AndAssignOperator()
                 : (OpAssignOperator) new GenericOpAssignOperator();
        }
    }
}
