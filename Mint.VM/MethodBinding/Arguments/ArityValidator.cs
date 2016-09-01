using System.Linq;
using System.Reflection;
using Mint.Reflection;
using Mint.Reflection.Parameters;

namespace Mint.MethodBinding.Arguments
{
    internal class ArityValidator
    {
        public ArgumentBundle Arguments { get; }

        public MethodInfo Method { get; }

        private ParameterCounter Counter { get; }

        public ArityValidator(ArgumentBundle arguments, MethodInfo methodInfo)
        {
            Arguments = arguments;
            Method = methodInfo;
            Counter = methodInfo.GetParameterCounter();
        }

        public void Validate()
        {
            var message = ValidateInternal();
            if(message != null)
            {
                throw new ArgumentError(message);
            }
        }

        public bool IsValid() => ValidateInternal() != null;

        private string ValidateInternal()
        {
            if(!Counter.HasKeywords)
            {
                return ValidateInclusiveArity();
            }

            if(Arguments.HasKeyArguments || Arguments.Splat.LastOrDefault() is Hash)
            {
                // keywords are bound first to required args, if there aren't enough,
                // and thus, included for arity purposes.
                // only after all required parameters are bound, is the last argument regarded as keywords,
                // and thus, excluded for arity purposes.

                return ValidateExclusiveArity()
                    ?? ValidateMissingKeywords()
                    ?? ValidateUnknownKeywords();
            }

            return ValidateInclusiveArity()
                ?? ValidateMissingKeywords();
        }

        private string ValidateInclusiveArity() =>
            Counter.Arity.Include(Arguments.Arity) ? null : WrongArityMessage(Arguments.Arity);

        private string ValidateExclusiveArity()
        {
            var arity = Arguments.Arity;
            return Counter.Arity.Minimum <= arity && --arity <= Counter.Arity.Maximum
                 ? null
                 : WrongArityMessage(arity);
        }

        private string WrongArityMessage(int arity) =>
            $"wrong number of arguments (given {arity}, expected {Counter.Arity})";

        private string ValidateMissingKeywords()
        {
            var parameters = Method.GetParameters();
            var requiredKeys = (
                from p in parameters
                let name = new Symbol(p.Name)
                where p.IsKeyRequired() && !Arguments.Keywords.HasKey(name)
                select name
            ).ToArray();

            if(requiredKeys.Length == 0)
            {
                return null;
            }

            var plural = requiredKeys.Length == 1 ? "" : "s";
            var keywords = string.Join(", ", requiredKeys);
            return $"missing keyword{plural}: {keywords}";
        }

        private string ValidateUnknownKeywords()
        {
            if(Counter.HasKeyRest)
            {
                return null;
            }

            var parameters = Method.GetParameters();
            var parameterNames =
                from p in parameters
                where p.IsKey()
                select new Symbol(p.Name)
            ;

            var unknownKeys = Arguments.Keywords.Keys.Where(p => p is Symbol).Cast<Symbol>()
                .Except(parameterNames).ToArray();

            if(unknownKeys.Length == 0)
            {
                return null;
            }

            var plural = unknownKeys.Length == 1 ? "" : "s";
            var keywords = string.Join(", ", unknownKeys);
            return $"unknown keyword{plural}: {keywords}";
        }
    }
}
