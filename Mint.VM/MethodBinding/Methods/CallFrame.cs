using System;
using System.Collections.Generic;

namespace Mint.MethodBinding.Methods
{
    public class CallFrame
    {
        [ThreadStatic]
	    public static CallFrame CurrentFrame = null;

        public iObject Instance { get; }

        public IList<LocalVariable> Arguments { get; }

        public IList<LocalVariable> Locals { get; }

        public CallFrame Caller { get; }

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

            if(locals == null)
            {
                locals = System.Array.Empty<LocalVariable>();
            }

            Instance = instance;
            Arguments = arguments;
            Locals = locals;
            Caller = caller;
        }

        public static CallFrame PushNewFrame(iObject instance,
                                             IList<LocalVariable> arguments,
                                             IList<LocalVariable> locals) =>
            CurrentFrame = new CallFrame(instance, arguments, locals, CurrentFrame);

        public static void PopFrame()
        {
            if(CurrentFrame != null)
            {
                CurrentFrame = CurrentFrame.Caller;
            }
        }
    }
}
