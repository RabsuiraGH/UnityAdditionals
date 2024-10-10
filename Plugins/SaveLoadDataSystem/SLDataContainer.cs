using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Utility.SaveLoad
{
    [Serializable]
    public class SLDataContainer : IData
    {
        private string _saveKey = string.Empty;

        public string SaveKey
        {
            get => _saveKey;
            set
            {
                if (_saveKey == string.Empty)
                    _saveKey = value;
            }
        }
        public string PathToPrefab = string.Empty;
        public string ScenePath = string.Empty;
        public bool DestroyCompletely = false;

        public SLDataContainer(string saveKey)
        {
            _saveKey = saveKey;
        }

        public void SetData(GameObject gameObject, string pathToObject, string scenePathT)
        {
            if (_saveKey == string.Empty)
                _saveKey = gameObject.name + "_" + Guid.NewGuid().ToString();
            PathToPrefab = pathToObject;
            ScenePath = scenePathT;
        }

        public void SetData(string saveKey, string pathToObject, string scenePathT)
        {
            if (_saveKey == string.Empty)
                _saveKey = saveKey;
            PathToPrefab = pathToObject;
            ScenePath = scenePathT;
        }

        public void OverrideSaveKey(string newSaveKey)
        {
            _saveKey = newSaveKey;
        }

        public string GetSaveKey()
        {
            return _saveKey;
        }

        public string GetPrefabPath()
        {
            return PathToPrefab;
        }

        public string GetScenePath()
        {
            return ScenePath;
        }

        public bool GetPermissionForDelete()
        {
            return DestroyCompletely;
        }

        public static T LoadContainer<T>(string savekey, Action callback = null) where T : SLDataContainer
        {
            var result = SaveLoadUtility<T>.LoadById(SceneManager.GetActiveScene().name, savekey);

            if (result == null) return default;

            callback?.Invoke();
            result.SaveKey = savekey;

            return result;
        }

        public void SaveDefaultData<T>(Action<bool> callback = null) where T : SLDataContainer
        {
            SaveLoadUtility<T>.SaveData((T)this, callback);
        }
    }
}