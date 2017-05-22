using System;

namespace Mint.Reflection
{
    public class DelegateMetadata
    {
        public DelegateMetadata(Delegate lambda, MethodMetadata method = null)
        {
            Lambda = lambda ?? throw new ArgumentNullException(nameof(lambda));

            if(method != null && lambda.Method != method.Method) throw new ArgumentException(nameof(method));

            Method = method ?? new MethodMetadata(lambda.Method);

            var instancePosition = Method.HasClosure ? 1 : 0;
            InstanceType = Method.Method.GetParameters()[instancePosition].ParameterType;
        }


        public Delegate Lambda { get; }
        public MethodMetadata Method { get; }
        public Type InstanceType { get; }
    }
}
