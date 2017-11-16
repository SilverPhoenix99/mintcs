using System;

namespace Mint
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class RubyModuleAttribute : Attribute
    {
        public string ModuleName { get; }

        public RubyModuleAttribute(string moduleName = null)
        {
            ModuleName = moduleName;
        }
    }
}