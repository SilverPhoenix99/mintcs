using System;

namespace Mint.MethodBinding
{
    public sealed partial class ClrMethodBinder : MethodBinder
    {
        internal enum ParameterKind
        {
            Required, // iObject a
            Optional, // iObject a = default(iObject)
            Params,   // params iObject[] a
            Ref,      // ref iObject a
            Out,      // out iObject a
        }

        internal class Parameter
        {
            public Parameter(Symbol name, Type type, ParameterKind kind)
            {
                Name = name;
                Type = type;
                Kind = kind;
            }

            public Symbol        Name { get; }
            public Type          Type { get; }
            public ParameterKind Kind { get; }
        }
    }
}