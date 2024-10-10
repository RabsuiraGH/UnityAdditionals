using Core.CustomInspector;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.CustomInspector
{
    [CustomEditor(typeof(Object), true, isFallback = true)]
    public class RequireComponentsInHierarchyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var targetObject = (MonoBehaviour)target;

            //DrawDefaultInspector();

            var attributes = targetObject.GetType().GetCustomAttributes(typeof(RequireComponentInHierarchy), true);
            foreach (RequireComponentInHierarchy attribute in attributes)
            {
                foreach (var componentType in attribute.ComponentTypes)
                {
                    if (FindComponentInHierarchy(targetObject.gameObject, componentType) == null)
                    {
                        EditorGUILayout.HelpBox($"Required component '{componentType.Name}' not found in the hierarchy.", MessageType.Warning);
                    }
                }
            }
        }

        private Component FindComponentInHierarchy(GameObject gameObject, System.Type componentType)
        {
            Component component = gameObject.GetComponent(componentType);
            if (component != null) return component;

            Transform parent = gameObject.transform.parent;
            while (parent != null)
            {
                component = parent.GetComponent(componentType);
                if (component != null) return component;
                parent = parent.parent;
            }

            foreach (Transform child in gameObject.transform)
            {
                component = child.GetComponent(componentType);
                if (component != null) return component;
                component = FindComponentInHierarchy(child.gameObject, componentType);
                if (component != null) return component;
            }

            return null;
        }
    }
}