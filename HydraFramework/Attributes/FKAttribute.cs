using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydraFramework.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class FKAttribute : Attribute
    {
        public string Name { get; set; }


        public FKAttribute()
        {
            this.Name = "";
        }
    }
}
