using UnityEngine;

namespace Core.Utility.JsonSaver
{
    public static class PathCombiner
    {
        public static string BasePath { get => _basePath; }
        private static readonly string _basePath = Application.persistentDataPath + "/";

        public static string GetBasePath(string additionalPath)
        {
            return _basePath + additionalPath + ".json";
        }
    }
}