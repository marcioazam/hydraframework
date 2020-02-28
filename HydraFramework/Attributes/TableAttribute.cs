using System;

namespace HydraFramework.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public TableAttribute()
        {
            this.Name = "";
        }
    }
}
