using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mint.Compilation
{
    internal class CompilerBinding
    {
        private readonly Dictionary<string, int> localsIndexes;

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

        public Expression NewBinding(Expression receiver) => Mint.Binding.Expressions.New(receiver);
    }
}