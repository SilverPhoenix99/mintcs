using System;
using System.Collections.Generic;

namespace Mint.MethodBinding.Methods
{
    public class CallFrame
    {
        [ThreadStatic]
	    public static CallFrame CurrentFrame;

        public iObject Instance { get; }

        public IList<LocalVariable> Arguments { get; }

        public IList<LocalVariable> Locals { get; }

        public CallFrame Caller { get; }

        public Module Module => Instance as Module ?? Instance.Class;

        public CallFrame(iObject instance,
                         IList<LocalVariable> arguments,
                         IList<LocalVariable> locals = null,
                         CallFrame caller = null)
        {
            if(instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if(arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            Instance = instance;
            Arguments = arguments;
            Locals = locals ?? System.Array.Empty<LocalVariable>();
            Caller = caller;
        }

        public static CallFrame PushNewFrame(iObject instance,
                                             IList<LocalVariable> arguments,
                                             IList<LocalVariable> locals) =>
            CurrentFrame = new CallFrame(instance, arguments, locals, CurrentFrame);

        public static void PopFrame() => CurrentFrame = CurrentFrame?.Caller;
    }
}
