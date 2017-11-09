using System;

namespace Mint
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class RubyMethodAttribute : Attribute
    {
        public string MethodName { get; }

        public RubyMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}