using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Mint.Compilation.Scopes;
using Mint.MethodBinding.Methods;
using Mint.Parse;
using Mint.Reflection.Parameters;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class MethodCompiler : CompilerComponentBase
    {
        private Ast<Token> LeftNode => Node[0];

        private string Name => Node[1].Value.Value;

        private Ast<Token> ParametersNode => Node[2];

        private Ast<Token> BodyNode => Node[3];

        public MethodCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
	    {
            var name = Constant(new Symbol(Name));
            var module = CompileModule();
            var moduleVar = Variable(typeof(Module), "module");
            var parameters = CompileParameters();
            var lambda = BuildDelegate(parameters);

            return Block(
                typeof(iObject),
                new[] { moduleVar },
                Assign(moduleVar, module),
                Module.Expressions.DefineMethod(
                    moduleVar,
                    DelegateMethodBinder.Expressions.New(name, moduleVar, Constant(lambda))
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
                    return new Parameter(typeof(Array), name, ParameterKind.KeyOptional, value);
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

        private Delegate BuildDelegate(Parameter[] parameters)
        {
            var scope = new MethodScope(Compiler);

            foreach(var parameter in parameters)
            {
                scope.AddPreInitializedVariable(parameter.Name, parameter.Local);
            }

            var instance = scope.Instance as ParameterExpression;
            var arguments = NewArrayInit(typeof(LocalVariable), parameters.Select(CompileLocalVariable));

            Compiler.StartScope(scope);

            try
            {
                var body = BodyNode.Accept(Compiler);

                body = Block(
                    typeof(iObject),
                    parameters.Select(_ => _.Local),
                    CallFrame.Expressions.Push(instance, arguments),
                    TryFinally(body, CallFrame.Expressions.Pop())
                );

                body = scope.CompileBody(body);

                var lambdaParameters = new[] { instance }.Concat(parameters.Select(_ => _.Param));
                var lambdaExpression = Expression.Lambda(body, Name, lambdaParameters);
                var lambda = lambdaExpression.Compile();

                var parameterInfos = lambda.Method.GetParameters();

                parameters.Zip(parameterInfos,
                    (p, i) => TypeDescriptor.AddAttributes(i, p.Kind.GetAttributes())
                ).All(_ => true); // force run

                return lambda;
            }
            finally
            {
                Compiler.EndScope();
            }
        }

        private static Expression CompileLocalVariable(Parameter parameter)
        {
            var expression = parameter.CompileLocalVariable();
            return Assign(parameter.Local, expression);
        }

        private class Parameter
        {
            public readonly Type Type;
            public readonly Symbol Name;
            public readonly ParameterKind Kind;
            public readonly Expression DefaultValue;
            public readonly ParameterExpression Param;
            public readonly ParameterExpression Local;

            public Parameter(Type type, string name, ParameterKind kind, Expression defaultValue = null)
            {
                Type = type;
                Name = new Symbol(name);
                Kind = kind;
                DefaultValue = defaultValue;
                Param = Parameter(type, name);
                Local = Variable(typeof(iObject), name);
            }

            public Expression CompileLocalVariable()
            {
                var value = CompileInitialValue();
                return LocalVariable.Expressions.New(Constant(Name), value);
            }

            private Expression CompileInitialValue() =>
                DefaultValue == null ? (Expression) Param : Coalesce(Param, DefaultValue);
        }
    }
}
