using System;

namespace Mint.MethodBinding
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class OptionalAttribute : Attribute
    { }
}