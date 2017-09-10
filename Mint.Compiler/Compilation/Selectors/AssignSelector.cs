using Mint.Parse;
using Mint.Compilation.Components;
using Mint.Compilation.Components.Operators;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Selectors
{
    internal class AssignSelector : ComponentSelectorBase
    {
        private const string OR_OP = "||";
        private const string AND_OP = "&&";

        private SyntaxNode LeftNode => Node[0];

        public AssignSelector(Compiler compiler) : base(compiler)
        { }

        public override CompilerComponent Select()
        {
            var operatorCompiler = CreateOperator(Node.Token);

            switch(LeftNode.Token.Type)
            {
                case tIDENTIFIER:
                    return new AssignLocalVariableCompiler(Compiler, operatorCompiler);

                case tIVAR:
                    return new AssignInstanceVariableCompiler(Compiler, operatorCompiler);

                case kDOT:
                    return new AssignPropertyCompiler(Compiler, operatorCompiler);

                case kLBRACK2:
                    return new AssignIndexerCompiler(Compiler, operatorCompiler);

                case tCONSTANT:
                    return new AssignConstantCompiler(Compiler, operatorCompiler);
            }

            throw new System.NotImplementedException();
        }

        private static AssignOperator CreateOperator(Token token)
        {
            if(token.Type == kASSIGN)
            {
                return new SimpleAssignOperator();
            }

            if(token.Type != tOP_ASGN)
            {
                throw new System.NotImplementedException();
            }

            return token.Text == OR_OP ? new OrAssignOperator()
                 : token.Text == AND_OP ? new AndAssignOperator()
                 : (AssignOperator) new GenericOpAssignOperator();
        }
    }
}
