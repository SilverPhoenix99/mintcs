using System;
using System.Linq.Expressions;

namespace Mint.Compilation.Scopes.Variables
{
    public class HiddenVariable
    {
        public ParameterExpression Variable { get;  }

        public Expression InitialValue { get; }

        public HiddenVariable(ParameterExpression variable, Expression initialValue)
        {
            if(variable == null) throw new ArgumentNullException(nameof(variable));
            if(initialValue == null) throw new ArgumentNullException(nameof(initialValue));

            Variable = variable;
            InitialValue = initialValue;
        }

        public Expression CompileInitialize() => Expression.Assign(Variable, InitialValue);
    }
}
