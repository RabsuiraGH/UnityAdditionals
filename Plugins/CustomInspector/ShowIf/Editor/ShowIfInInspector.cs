using UnityEditor;
using UnityEngine;
using Core.CustomInspector;

namespace CoreEditor.CustomInspector
{
    

    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute attribute = (ShowIfAttribute)base.attribute;

            SerializedProperty conditionProperty = property.serializedObject.FindProperty(attribute.conditionalFieldName);

            if (conditionProperty == null)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            bool showProperty = conditionProperty.boolValue;

            if (showProperty)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute attribute = (ShowIfAttribute)base.attribute;

            SerializedProperty conditionProperty = property.serializedObject.FindProperty(attribute.conditionalFieldName);

            if (conditionProperty == null)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }

            bool showProperty = conditionProperty.boolValue;

            if (showProperty)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                return 0;
            }
        }
    }
}