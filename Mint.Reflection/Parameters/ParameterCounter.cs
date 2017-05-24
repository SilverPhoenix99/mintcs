using System;
using System.Linq;

namespace Mint.Reflection.Parameters
{
    public partial class ParameterCounter : Attribute
    {
        // parameters, if specified, will follow this order:
        // Required, Optional, Rest, Required, (KeyRequired | KeyOptional), KeyRest, Block


        public ParameterCounter(MethodMetadata method)
        {
            ParameterState initialState = new RequiredPrefixState(this);
            method.Parameters.Aggregate(initialState, (state, parameter) => state.Parse(parameter));
        }


        public int PrefixRequired { get; private set; }
        public int Optional { get; private set; }
        public bool HasRest { get; private set; }
        public int SuffixRequired { get; private set; }
        public int KeyRequired { get; private set; }
        public int KeyOptional { get; private set; }
        public bool HasKeyRest { get; private set; }
        public bool HasBlock { get; private set; }
        public bool HasKeywords => KeyRequired > 0 || KeyOptional > 0 || HasKeyRest;
        public int Required => PrefixRequired + SuffixRequired;
        

        public Arity Arity
        {
            get
            {
                var min = PrefixRequired + SuffixRequired;
                var max = HasRest ? int.MaxValue : min + Optional;
                return new Arity(min, max);
            }
        }
    }
}