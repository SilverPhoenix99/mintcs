using System.Linq;
using Mint.Reflection.Parameters;
using Mint.Reflection;

namespace Mint.MethodBinding.Arguments
{
    internal class ArityValidator
    {
        private readonly ArgumentBundle arguments;
        private readonly MethodMetadata method;
        private readonly ParameterCounter counter;


        public ArityValidator(ArgumentBundle arguments, MethodMetadata method)
        {
            this.arguments = arguments;
            this.method = method;
            counter = method.ParameterCounter;
        }


        public void Validate()
        {
            var message = ValidateInternal();
            if(message != null)
            {
                throw new ArgumentError(message);
            }
        }


        public bool IsValid()
            => ValidateInternal() == null;


        private string ValidateInternal()
        {
            if(!counter.HasKeywords)
            {
                return ValidateInclusiveArity();
            }

            if(arguments.HasKeyArguments || arguments.Splat.LastOrDefault() is Hash)
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


        private string ValidateInclusiveArity()
            => counter.Arity.Include(arguments.Arity) ? null : WrongArityMessage(arguments.Arity);


        private string ValidateExclusiveArity()
        {
            var arity = arguments.Arity;
            return counter.Arity.Minimum <= arity && --arity <= counter.Arity.Maximum
                 ? null
                 : WrongArityMessage(arity);
        }


        private string WrongArityMessage(int arity)
            => $"wrong number of arguments (given {arity}, expected {counter.Arity})";


        private string ValidateMissingKeywords()
        {
            var requiredKeys = (
                from p in method.Parameters
                let name = new Symbol(p.Name)
                where p.IsKeyRequired && !arguments.Keywords.HasKey(name)
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
            if(counter.HasKeyRest)
            {
                return null;
            }

            var parameterNames =
                from p in method.Parameters
                where p.IsKey
                select new Symbol(p.Name)
            ;

            var unknownKeys = arguments.Keywords.Keys.OfType<Symbol>()
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
