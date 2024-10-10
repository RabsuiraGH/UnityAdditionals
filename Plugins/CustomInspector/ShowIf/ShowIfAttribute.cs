using UnityEngine;

namespace Core.CustomInspector
{
    public class ShowIfAttribute : PropertyAttribute
    {
        public readonly string conditionalFieldName;

        public ShowIfAttribute(string conditionalFieldName)
        {
            this.conditionalFieldName = conditionalFieldName;
        }
    }
}
