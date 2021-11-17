using System;

namespace CSCore.Utils
{
    [RemoveObj]
    [AttributeUsage(AttributeTargets.Method)]
    internal class CSCalliAttribute : Attribute
    {
        public CSCalliAttribute()
        {
        }
    }
}