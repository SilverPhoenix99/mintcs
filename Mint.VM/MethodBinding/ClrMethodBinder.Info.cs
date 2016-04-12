using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace Mint.MethodBinding
{
    public sealed partial class ClrMethodBinder : MethodBinder
    {
        private class Info
        {
            public Info(MethodInfo method)
            {
                Method = method;
                Parameters = InitParameters();
            }

            public MethodInfo  Method     { get; }
            public Parameter[] Parameters { get; }

            private Parameter[] InitParameters()
            {
                return new Parameter[0];
                //TODO throw new NotImplementedException();
            }
        }
    }
}