using System;

namespace HydraFramework.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class PKAttribute : Attribute
    {
        public string Name { get; set; }

        public bool Identity { get; set; }

        public PKAttribute()
        {
            this.Name = "";
            this.Identity = true;
        }
    }
}
