using System;

namespace CoviIDApiCore.V1.Attributes {

    [AttributeUsage(AttributeTargets.Property)]
    public class Encrypted : Attribute
    {
        public Encrypted(bool serverManaged = false) {
            this.serverManaged = serverManaged;
        }

        public bool serverManaged { get; set; }
    }

}
