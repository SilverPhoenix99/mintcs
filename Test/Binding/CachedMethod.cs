using System;
//using System.Linq.Expressions;
using Mint;
//using static System.Linq.Expressions.Expression;

namespace Mint.Binding
{
    class CachedMethod
    {
        public CachedMethod(Method method)
        {
            Condition = method.Condition;
            // TODO
            // CompiledMethod = method.Compile();
        }

        public Condition Condition { get; }
        public Func<iObject, iObject[], iObject> CompiledMethod { get; }
    }
}