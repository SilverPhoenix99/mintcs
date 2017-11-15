using System;

namespace Mint
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class RubyClassAttribute : Attribute
    {
        public string ClassName { get; }
        public Type Superclass { get; set; }

        public RubyClassAttribute(string className = null)
        {
            ClassName = className;
            Superclass = typeof(Object);
        }
    }
}