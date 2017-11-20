using System;
using System.Linq;
using System.Linq.Expressions;
using Mint.Compilation.Scopes;
using Mint.MethodBinding;
using Mint.Parse;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal partial class MethodCompiler
    {
        private class DelegateEmitter
        {
            private readonly Compiler Compiler;
            private readonly string Name;
            private readonly Parameter[] Parameters;
            private readonly Scope Scope;

            private ParameterExpression Instance => Scope.Instance as ParameterExpression;

            private SyntaxNode BodyNode => Compiler.CurrentNode[3];

            public DelegateEmitter(Compiler compiler, string name, Parameter[] parameters)
            {
                Compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
                Scope = new MethodScope(Compiler);
            }

            public Delegate Compile()
            {
                foreach(var parameter in Parameters)
                {
                    Scope.AddPreInitializedVariable(parameter.Name, parameter.Local);
                }

                Compiler.StartScope(Scope);

                try
                {
                    var body = CompileLambdaBody();
                    var lambdaParameters = new[] { Instance }.Concat(Parameters.Select(_ => _.Param));
                    var lambda = Lambda(body, Name, lambdaParameters);
                    return lambda.Compile();
                }
                finally
                {
                    Compiler.EndScope();
                }
            }

            private Expression CompileLambdaBody()
            {
                var localVariables = Parameters.Select(CompileLocalVariable);
                var arguments = NewArrayInit(typeof(LocalVariable), localVariables);

                var body = BodyNode.Accept(Compiler);

                body = Block(
                    typeof(iObject),
                    Parameters.Select(_ => _.Local),
                    CallFrame.Expressions.Push(null, Instance, arguments),
                    TryFinally(body, CallFrame.Expressions.Pop())
                );

                return Scope.CompileBody(body);
            }

            private static Expression CompileLocalVariable(Parameter parameter)
            {
                var expression = parameter.CompileLocalVariable();
                return Assign(parameter.Local, expression);
            }
        }
    }
}
