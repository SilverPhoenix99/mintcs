using System;

// ReSharper disable once CheckNamespace
namespace Mint.Reflection.Parameters.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class OptionalAttribute : Attribute
    { }


    [AttributeUsage(AttributeTargets.Parameter)]
    public class RestAttribute : Attribute
    { }


    [AttributeUsage(AttributeTargets.Parameter)]
    public class KeyAttribute : Attribute
    { }


    [AttributeUsage(AttributeTargets.Parameter)]
    public class BlockAttribute : Attribute
    { }
}
