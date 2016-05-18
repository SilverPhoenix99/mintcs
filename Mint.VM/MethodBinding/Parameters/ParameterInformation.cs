using System.Collections.Generic;
using System.Reflection;

namespace Mint.MethodBinding.Parameters
{
    public partial class ParameterInformation
    {
        // parameters, if specified, will follow this order:
        // Required, Optional, Rest, Required, (KeyRequired | KeyOptional), KeyRest, Block

        public int PrefixRequired { get; private set; }
        public int Optional { get; private set; }
        public bool HasRest { get; private set; }
        public int SuffixRequired { get; private set; }
        public int KeyRequired { get; private set; }
        public int KeyOptional { get; private set; }
        public bool HasKeyRest { get; private set; }
        public bool HasBlock { get; private set; }

        public Range Arity
        {
            get
            {
                long min = PrefixRequired + SuffixRequired + KeyRequired;
                long max = HasRest || HasKeyRest
                         ? long.MaxValue
                         : min + Optional + KeyOptional + (HasBlock ? 1 : 0);

                return new Range(new Fixnum(min), new Fixnum(max));
            }
        }

        public ParameterInformation(IEnumerable<ParameterInfo> parameterInfos)
        {
            ParameterState state = new RequiredPrefixState(this);

            foreach(var parameterInfo in parameterInfos)
            {
                state = state.Parse(parameterInfo);
            }
        }
    }
}