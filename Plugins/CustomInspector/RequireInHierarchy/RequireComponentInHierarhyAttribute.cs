using System;

namespace Core.CustomInspector
{
    public class RequireComponentInHierarchy : Attribute
    {
        public System.Type[] ComponentTypes { get; }

        public RequireComponentInHierarchy(params System.Type[] componentTypes)
        {
            ComponentTypes = componentTypes;
        }
    }
}