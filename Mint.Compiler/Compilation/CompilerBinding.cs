using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Mint;
using Mint.Reflection;

namespace Mint.Compilation
{
    internal class CompilerBinding
    {
        private static readonly ConstructorInfo CTOR = Reflector.Ctor<Binding>(typeof(iObject));

        private static readonly MethodInfo GET_OR_CREATE_LOCAL =
            Reflector<Binding>.Method(_ => _.SetLocal(default(Symbol), default(iObject)));

        private static readonly MethodInfo SET_LOCAL =
            Reflector<Binding>.Method(_ => _.SetLocal(default(Symbol), default(iObject)));

        private Dictionary<string, int> localsIndexes;

        public Expression Binding { get; }

        public CompilerBinding()
        {
            localsIndexes = new Dictionary<string, int>();
            Binding = Expression.Parameter(typeof(Binding), "binding");
        }

        public int GetOrCreateLocal(string name)
        {
            int index;
            if(!localsIndexes.TryGetValue(name, out index))
            {
                localsIndexes[name] = index = localsIndexes.Count;
            }

            return index;
        }

        public Expression NewBinding(Expression receiver) => Expression.New(CTOR, receiver);
    }
}