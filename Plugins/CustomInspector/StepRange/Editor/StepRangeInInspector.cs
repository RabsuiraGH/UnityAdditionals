using Core.CustomInspector;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.CustomInspector
{
    [CustomPropertyDrawer(typeof(StepRangeAttribute))]
    public class StepRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            StepRangeAttribute range = attribute as StepRangeAttribute;

            if (property.propertyType == SerializedPropertyType.Float)
            {
                EditorGUI.Slider(position, property, range.minValue, range.maxValue, label);
                property.floatValue = Mathf.Round(property.floatValue / range.step) * range.step;
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUI.IntSlider(position, property, (int)range.minValue, (int)range.maxValue, label);
                property.intValue = Mathf.RoundToInt(property.intValue / range.step) * (int)range.step;
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use StepRange with float or int.");
            }
        }
    }
}