using System;
using Mint.MethodBinding;

namespace Mint
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class RubyMethodAttribute : Attribute
    {
        public string MethodName { get; }

        public Visibility Visibility { get; set; }

        public RubyMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }

        public bool Equals(RubyMethodAttribute obj) =>
            obj != null
            && obj.MethodName.Equals(MethodName)
            && obj.Visibility.Equals(Visibility)
        ;

        public override bool Equals(object obj) => Equals(obj as RubyMethodAttribute);
    }
}