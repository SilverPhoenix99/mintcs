using Mint.Parse;
using Mint.Binding.Arguments;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class AssignIndexerCompiler : CompilerComponentBase
    {
        // <left>.[*<args>] = <right>   =>   <left>.[]=(*<args>, <right>)

        private Ast<Token> LeftNode => Node[0][0];
        private Ast<Token> RightNode => Node[1];
        private Ast<Token> ArgumentsNode => Node[0][1];

        public AssignIndexerCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            Push(LeftNode);
            PushArguments();
            Push(RightNode);
        }

        private void PushArguments()
        {
            foreach(var argument in ArgumentsNode)
            {
                Push(argument);
            }
        }

        public override Expression Reduce()
        {
            var left = Pop();
            var arguments = PopArguments();
            var right = Pop();

            arguments = arguments.Concat(new[] { new InvocationArgument(ArgumentKind.Simple, right) });

            var visibility = GetVisibility(LeftNode);

            return CompilerUtils.Call(left, Symbol.ASET, visibility, arguments.ToArray());
        }

        private IEnumerable<InvocationArgument> PopArguments()
        {
            return from astArgument in ArgumentsNode
                   select AsArgumentKind(astArgument.Value.Type) into kind
                   let argument = Pop()
                   select new InvocationArgument(kind, argument);
        }

        private static ArgumentKind AsArgumentKind(TokenType type)
        {
            switch(type)
            {
                case kSTAR:
                    return ArgumentKind.Rest;

                case tLABEL: goto case kASSOC;
                case tLABEL_END: goto case kASSOC;
                case kASSOC:
                    return ArgumentKind.Key;

                case kDSTAR:
                    return ArgumentKind.KeyRest;

                case kLBRACE2: goto case kAMPER;
                case kDO: goto case kAMPER;
                case kAMPER:
                    return ArgumentKind.Block;

                default:
                    return ArgumentKind.Simple;
            }
        }
    }
}