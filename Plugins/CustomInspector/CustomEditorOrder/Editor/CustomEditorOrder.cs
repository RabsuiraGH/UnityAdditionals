using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreEditor.CustomInspector
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class CombinedEditor : Editor
    {   
        private Editor[] customEditors;

        private void OnEnable()
        {
            var targetObject = target;

            customEditors = new Editor[]
            {
            // In required order
            //CreateEditor(targetObject, typeof(NameOfEditor)),
            //CreateEditor(targetObject, typeof(NameOfEditor))
            };
        }

        public override void OnInspectorGUI()
        {
            foreach (var editor in customEditors)
            {
                editor.OnInspectorGUI();
            }
        }

        private Editor CreateEditor(MonoBehaviour target, Type editorType)
        {
            return Editor.CreateEditor(target, editorType);
        }
    }
}