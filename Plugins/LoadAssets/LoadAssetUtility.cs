using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

using namespace Core.Utility.AssetLoader
{
    public static class LoadAssetUtility
    {
        public static T Load<T>(string fullPath) where T : UnityEngine.Object
        {
            string cutPath = fullPath.Replace("Assets/Resources/", "").Split(".")[0];

            T resource = Resources.Load<T>(cutPath);

            if (resource == null)
            {
                Debug.LogError($"SCRIPT ISSUE: FILE IN ==={fullPath}=== DOESNT EXIST.\nLAST EXISTING FLODER <a href=\"{FindExistingPath(fullPath)}\">{FindExistingPath(fullPath)}</a>");
                return null;
            }
            return resource;
        }

        public static string FindExistingPath(string fullPath)
        {
            string[] folders = fullPath.Split('/');
            string currentPath = "";
            string lastExistingPath = "";

            foreach (string folder in folders)
            {
                currentPath = Path.Combine(currentPath, folder);

                if (Directory.Exists(currentPath))
                {
                    lastExistingPath = currentPath;
                }
                else
                {
                    break;
                }
            }

            return lastExistingPath;
        }

#if UNITY_EDITOR

    [MenuItem("Tools/LoadAssetUtility/LoadAllResources")]
    public static void LoadAllResources()
    {
        ILoadResources[] loadResourceComponents = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<ILoadResources>().ToArray();

        foreach (ILoadResources component in loadResourceComponents)
        {
            component.Load();
        }
    }

#endif

        public static GameObject Find(string objectName)
        {
            GameObject gameObject = GameObject.Find(objectName);
            if (gameObject == null)
            {
                Debug.LogError($"Object with name {objectName} does not exist in scene");
            }
            return gameObject;
        }
    }

    public interface ILoadResources
    {
        public void Load();
    }
}