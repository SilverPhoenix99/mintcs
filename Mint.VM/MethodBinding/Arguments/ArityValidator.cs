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
            if(!Counter.HasKeywords)
            {
                ValidateInclusiveArity();
                return;
            }

            if(Arguments.HasKeyArguments || Arguments.Splat.LastOrDefault() is Hash)
            {
                // keywords are bound first to required args, if there aren't enough,
                // and thus, included for arity purposes.
                // only after all required parameters are bound, is the last argument regarded as keywords,
                // and thus, excluded for arity purposes.

                ValidateExclusiveArity();
                ValidateMissingKeywords();
                ValidateUnknownKeywords();
                return;
            }

            ValidateInclusiveArity();
            ValidateMissingKeywords();

        }

        private void ValidateInclusiveArity()
        {
            if(!Counter.Arity.Include(Arguments.Arity))
            {
                ThrowWrongArityError(Arguments.Arity);
            }
        }

        private void ValidateExclusiveArity()
        {
            var arity = Arguments.Arity;
            if(arity < Counter.Arity.Minimum || --arity > Counter.Arity.Maximum)
            {
                ThrowWrongArityError(arity);
            }
        }

        private void ThrowWrongArityError(int arity)
        {
            var message = $"wrong number of arguments (given {arity}, expected {Counter.Arity})";
            throw new ArgumentError(message);
        }

        private void ValidateMissingKeywords()
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
                return;
            }

            var plural = requiredKeys.Length == 1 ? "" : "s";
            var keywords = string.Join(", ", requiredKeys);
            throw new ArgumentError($"missing keyword{plural}: {keywords}");
        }

        private void ValidateUnknownKeywords()
        {
            if(Counter.HasKeyRest)
            {
                return;
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
                return;
            }

            var plural = unknownKeys.Length == 1 ? "" : "s";
            var keywords = string.Join(", ", unknownKeys);
            throw new ArgumentError($"unknown keyword{plural}: {keywords}");
        }
    }
}
