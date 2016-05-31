using Mint.Binding.Arguments;
using Mint.Parse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class IndexerCompiler : CompilerComponentBase
    {
        // <left>[*<args>]   =>   <left>.[](*<args>)

        protected virtual Ast<Token> LeftNode => Node[0];
        protected virtual Ast<Token> ArgumentsNode => Node[1];

        public IndexerCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            Push(LeftNode);
            PushArguments();
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

            var visibility = CompilerUtils.GetVisibility(LeftNode);
            return CompilerUtils.Call(left, Symbol.AREF, visibility, arguments);
        }

        protected InvocationArgument[] PopArguments()
        {
            return (
                from astArgument in ArgumentsNode
                select AsArgumentKind(astArgument.Value.Type) into kind
                let argument = Pop()
                select new InvocationArgument(kind, argument)
            ).ToArray();
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
