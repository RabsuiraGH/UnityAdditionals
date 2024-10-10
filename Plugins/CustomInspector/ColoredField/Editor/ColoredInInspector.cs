using Core.CustomInspector;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.CustomInspector
{
    [CustomPropertyDrawer(typeof(ColoredInInspectorAttribute))]
    public class ColoredInInspectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ColoredInInspectorAttribute coloredAttribute = (ColoredInInspectorAttribute)attribute;

            Color originalColor = GUI.color;
            GUI.color = coloredAttribute.color;

            EditorGUI.PropertyField(position, property, label, true);

            GUI.color = originalColor;
        }
    }
}