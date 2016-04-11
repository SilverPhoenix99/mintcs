using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using Parameter = Mint.MethodBinding.ClrMethodBinder.Parameter;

namespace Mint.MethodBinding
{
    public sealed partial class ClrPropertyBinder : MethodBinder
    {
        private class Info
        {
            public Info(PropertyInfo property)
            {
                Property = property;
                Parameters = InitParameters();
            }

            public PropertyInfo Property   { get; }
            public Parameter[]  Parameters { get; }

            private Parameter[] InitParameters()
            {
                throw new NotImplementedException();
            }
        }
    }
}