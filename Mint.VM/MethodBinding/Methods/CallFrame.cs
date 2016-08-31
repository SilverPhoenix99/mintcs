using System;
using System.Collections.Generic;

namespace Mint.MethodBinding.Methods
{
    public class CallFrame
    {
        public iObject Instance { get; }

        public IList<LocalVariable> Arguments { get; }

        public IList<LocalVariable> Locals { get; }

        public CallFrame Caller { get; }

        public CallFrame(iObject instance, IList<LocalVariable> arguments, IList<LocalVariable> locals, CallFrame caller = null)
        {
            if(instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if(arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            if(locals == null)
            {
                throw new ArgumentNullException(nameof(locals));
            }

            Instance = instance;
            Arguments = arguments;
            Locals = locals;
            Caller = caller;
        }
    }
}
