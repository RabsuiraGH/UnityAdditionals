using UnityEngine;

namespace Core.CustomInspector
{
    public class ColoredInInspectorAttribute : PropertyAttribute
    {
        public Color color;

        public ColoredInInspectorAttribute(float r, float g, float b)
        {
            color = new Color(r, g, b);
        }

        public ColoredInInspectorAttribute()
        {
            color = Color.gray; //
        }
    }
}