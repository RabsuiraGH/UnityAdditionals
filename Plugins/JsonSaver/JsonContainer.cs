using System;
using UnityEngine;

namespace Core.Utility.JsonSaver
{
    [Serializable]
    public class JsonContainer : IJsonData
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

        public bool Delete = false;

        public JsonContainer(string saveKey)
        {
            _saveKey = saveKey;
        }

        public void OverrideSaveKey(string newSaveKey)
        {
            _saveKey = newSaveKey;
        }

        public bool GetPermissionForDelete()
        {
            return Delete;
        }

        public string GetSaveKey()
        {
            return _saveKey;
        }

        public static T LoadContainer<T>(string fullSavePath, string space, string saveKey, Action callback = null, bool encryption = false) where T : JsonContainer
        {
            var result = JsonSaver<T>.LoadById(space, saveKey, fullSavePath, encryption);

            if (result == null) return default;

            callback?.Invoke();
            result.SaveKey = saveKey;

            return result;
        }

        public void SaveDefaultData<T>(string fullSavePath, string space, Action<bool> callback = null) where T : JsonContainer
        {
            JsonSaver<T>.SaveData(fullSavePath, space, (T)this, callback);
        }
    }
}