using System;
using System.Linq.Expressions;
using Mint.MethodBinding;
using Mint.Reflection.Parameters;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal partial class MethodCompiler
    {
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
                Local = Variable(typeof(LocalVariable), $"localVariable«{name}»");
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
