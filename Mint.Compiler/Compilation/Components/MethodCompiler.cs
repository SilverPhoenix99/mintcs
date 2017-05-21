using System.Linq;
using System.Linq.Expressions;
using Mint.MethodBinding.Methods;
using Mint.Parse;
using Mint.Reflection.Parameters;
using static System.Linq.Expressions.Expression;
using Mint.Reflection;

namespace Mint.Compilation.Components
{
    internal partial class MethodCompiler : CompilerComponentBase
    {
        private Ast<Token> LeftNode => Node[0];

        private string Name => Node[1].Value.Value;

        private Ast<Token> ParametersNode => Node[2];

        public MethodCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
	    {
            var name = Constant(new Symbol(Name));
            var moduleVar = Variable(typeof(Module), "module");
            var module = CompileModule();
            var delegateMetadata = BuildDelegateMetadata();

            /*
             * var $module = <module>;
             * return (iObject) $module.DefineMethod(new DelegateMethodBinder(@name, $module, @delegateMetadata);
             */
            return Block(
                typeof(iObject),
                new[] { moduleVar },
                Assign(moduleVar, module),
                Module.Expressions.DefineMethod(
                    moduleVar,
                    DelegateMethodBinder.Expressions.New(name, moduleVar, moduleVar, Constant(delegateMetadata))
                ).Cast<iObject>()
            );
	    }

        private Expression CompileModule()
        {
            if(LeftNode.IsList)
            {
                return Compiler.CurrentScope.Module;
            }

            var instance = LeftNode.Accept(Compiler);
            return Object.Expressions.SingletonClass(instance).Cast<Module>();
        }

        private DelegateMetadata BuildDelegateMetadata()
        {
            var parameters = CompileParameters();

            var emitter = new DelegateEmitter(Compiler, Name, parameters);
            var lambda = emitter.Compile();

            var builder = new DelegateMetadataBuilder(lambda, Name, parameters);
            return builder.Build();
        }
        
        private Parameter[] CompileParameters() => ParametersNode.Select(CompileParameter).ToArray();

        private Parameter CompileParameter(Ast<Token> node)
        {
            switch(node.Value.Type)
            {
                case TokenType.kASSIGN:
                {
                    var name = node[0].Value.Value;
                    var value = node[1].Accept(Compiler);
                    return new Parameter(typeof(iObject), name, ParameterKind.Optional, value);
                }

                case TokenType.kSTAR:
                {
                    var name = node[0].Value.Value;
                    return new Parameter(typeof(Array), name, ParameterKind.Rest);
                }

                case TokenType.tLABEL:
                {
                    var name = node.Value.Value;
                    name = name.Remove(name.Length - 1);

                    if(node.List.Count == 0)
                    {
                        return new Parameter(typeof(iObject), name, ParameterKind.KeyRequired);
                    }

                    var value = node[0].Accept(Compiler);
                    return new Parameter(typeof(iObject), name, ParameterKind.KeyOptional, value);
                }

                case TokenType.kDSTAR:
                {
                    var name = node[0].Value.Value;
                    return new Parameter(typeof(Hash), name, ParameterKind.KeyRest);
                }

                case TokenType.kAMPER:
                {
                    var name = node[0].Value.Value;
                    return new Parameter(typeof(Proc), name, ParameterKind.Block);
                }

                default:
                {
                    var name = node.Value.Value;
                    return new Parameter(typeof(iObject), name, ParameterKind.Required);
                }
            }
        }
    }
}
