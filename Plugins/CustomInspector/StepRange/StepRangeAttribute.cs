using UnityEngine;

namespace Core.CustomInspector
{
    public class StepRangeAttribute : PropertyAttribute
    {
        public float minValue;
        public float maxValue;
        public float step;

        public StepRangeAttribute(float minValue, float maxValue, float step)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
        }
    }
}
