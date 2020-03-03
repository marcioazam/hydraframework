using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydraFramework.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class AutoJoinAttribute : Attribute
    {
        public Type ReferencedType;

        public AutoJoinAttribute(Type ReferencedType)
        {
            this.ReferencedType = ReferencedType;
        }
    }
}