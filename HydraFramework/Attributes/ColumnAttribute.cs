using System;

namespace HydraFramework.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public ColumnAttribute()
        {
            this.Name = "";
        }
    }
}
