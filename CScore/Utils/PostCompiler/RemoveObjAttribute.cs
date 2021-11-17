using System;

namespace CSCore.Utils
{
    [RemoveObj]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    internal class RemoveObjAttribute : Attribute
    {
    }
}