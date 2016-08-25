using System.Collections.Generic;

namespace Mint
{
    public class Binding : BaseObject
    {
        private Dictionary<Symbol, int> localsIndexes;

        private Array locals;

        public readonly iObject Receiver;

        public Module Module => Receiver as Module ?? Receiver.Class;

        public Binding(iObject receiver) : base(Class.BINDING)
        {
            Receiver = receiver;
            localsIndexes = new Dictionary<Symbol, int>();
            locals = new Array();
        }

        public bool IsLocalDefined(Symbol local) => localsIndexes.ContainsKey(local);

        public iObject GetLocal(Symbol local)
        {
            var index = GetLocalIndexOrThrow(local);
            return locals[index];
        }

        public int GetLocalIndexOrThrow(Symbol local)
        {
            int index;
            if(localsIndexes.TryGetValue(local, out index))
            {
                return index;
            }
            throw new NameError($"local variable `{local.Name}' not defined for {this}");
        }

        public void SetLocal(Symbol local, iObject value)
        {
            var index = GetOrCreateLocalIndex(local);
            SetLocal(index, value);
        }

        public int GetOrCreateLocalIndex(Symbol local)
        {
            int index;
            if(!localsIndexes.TryGetValue(local, out index))
            {
                localsIndexes[local] = index = locals.Count;
                locals[index] = new NilClass();
            }
            return index;
        }

        public void SetLocal(int index, iObject value)
        {
            locals[index] = value ?? new NilClass();
        }
    }
}