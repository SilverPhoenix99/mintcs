using System;

namespace Mint
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class RubyClassAttribute : Attribute
    {
        public string ClassName { get; }

        public RubyClassAttribute(string className = null)
        {
            ClassName = className;
        }
    }
}