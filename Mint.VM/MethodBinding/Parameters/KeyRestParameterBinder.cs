using System.Collections.Generic;
using System.Linq;
using Mint.MethodBinding.Arguments;
using Mint.Reflection;

namespace Mint.MethodBinding.Parameters
{
    internal class KeyRestParameterBinder : ParameterBinder
    {
        public KeyRestParameterBinder(MethodMetadata method, ParameterMetadata parameter)
            : base(method, parameter)
        { }


        public override iObject Bind(ArgumentBundle bundle)
        {
            var parameters = Method.Parameters.Where(p => p.IsKeyRequired || p.IsKeyOptional);
            var keysToExclude = new HashSet<Symbol>(parameters.Select(p => new Symbol(p.Name)));

            var keys = new HashSet<Symbol>(bundle.Keywords.Keys.Cast<Symbol>());
            keys.RemoveWhere(key => keysToExclude.Contains(key));

            var rest = new Hash(keys.Count);

            foreach(var key in keys)
            {
                rest[key] = bundle.Keywords[key];
            }

            return rest;
        }
    }
}