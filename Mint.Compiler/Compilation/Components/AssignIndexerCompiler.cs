using Mint.Parse;
using Mint.Binding.Arguments;
using System.Collections.Generic;
using System.Linq.Expressions;
using static Mint.Parse.TokenType;

namespace Mint.Compilation.Components
{
    internal class AssignIndexerCompiler : CompilerComponentBase
    {
        // <left>.[*<args>] = <right>   =>   <left>.[]=(*<args>, <right>)

        public AssignIndexerCompiler(Compiler compiler) : base(compiler)
        { }

        public override void Shift()
        {
            Push(Node[0][0]);
            PushArguments();
            Push(Node[1]);
        }

        public void PushArguments()
        {
            foreach(var argument in Node[0][1])
            {
                switch(argument.Value.Type)
                {
                    case kAMPER: goto case kSTAR;
                    case kDSTAR: goto case kSTAR;
                    case kSTAR:
                        Push(argument[0]);
                        break;

                    case tLABEL:
                        Push(argument);
                        Push(argument[0]);
                        break;

                    case tLABEL_END: goto case kASSOC;
                    case kASSOC:
                        Push(argument[0]);
                        Push(argument[1]);
                        break;

                    default:
                        Push(argument);
                        break;
                }
            }
        }

        public override Expression Reduce()
        {
            var left = Pop();
            var arguments = PopArguments();
            var right = Pop();

            // TODO create invocation expression
            throw new System.NotImplementedException();
        }

        private IEnumerable<InvocationArgument> PopArguments()
        {
            var arguments = new List<InvocationArgument>(Node[0][1].List.Count);

            foreach(var argumentNode in Node[0][1])
            {
                var argument = PopArgument(argumentNode.Value.Type);
                arguments.Add(argument);
            }

            return arguments;
        }

        private InvocationArgument PopArgument(TokenType type)
        {
            switch(type)
            {
                case tLABEL: goto case kASSOC;
                case kASSOC:
                {
                    var label = Pop();
                    var value = Pop();
                    var argument = CompilerUtils.NewArray(label, value);
                    return new InvocationArgument(ArgumentKind.Key, argument);
                }

                case tLABEL_END:
                {
                    var label = Pop();
                    var value = Pop();

                    if(value is BlockExpression)
                    {
                        //value = CompilerUtils.StringConcat(((BlockExpression) value).Expressions);
                        //return CompilerUtils.NewSymbol(value);
                        // TODO String.Concat(Block.Expressions)
                        throw new System.NotImplementedException();
                    }

                    var argument = CompilerUtils.NewArray(label, value);
                    return new InvocationArgument(ArgumentKind.Key, argument);
                }

                case kSTAR:
                {
                    var argument = Pop();
                    return new InvocationArgument(ArgumentKind.Rest, argument);
                }

                case kDSTAR:
                {
                    var argument = Pop();
                    return new InvocationArgument(ArgumentKind.KeyRest, argument);
                }

                case kAMPER:
                {
                    var argument = Pop();
                    return new InvocationArgument(ArgumentKind.Block, argument);
                }

                default:
                {
                    var argument = Pop();
                    return new InvocationArgument(ArgumentKind.Simple, argument);
                }
            }
        }
    }
}