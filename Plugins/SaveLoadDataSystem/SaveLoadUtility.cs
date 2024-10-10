using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Core.Utility.SaveLoad
{
    // DEBUG_MODE_SAVELOAD
    public static class SaveLoadUtility<T> where T : IData
    {
        private static readonly string _savePath = Application.persistentDataPath + "/" + "savefile.json";

        private static readonly bool _useEncryption = false;

        private static readonly string _encryptingCodeWord = "MyBaseEncryptionCode";

        /// <summary>
        /// Save data container in current scene space
        /// </summary>
        public static void SaveData(T data, Action<bool> callback = null)
        {
            SaveData(ParseSceneName(data.GetScenePath()), data, callback);
        }

        /// <summary>
        /// Save data container in specific scene space
        /// </summary>
        public static void SaveData(string scene, T data, Action<bool> callback = null)
        {
            // parses name from path and loads full jobjec
            scene = ParseSceneName(scene);
            JObject jsonObject = TryLoadSaveFile();

            // if no scene space in save, creates new one
            if (jsonObject[scene] == null)
            {
                jsonObject[scene] = new JArray();
            }

            // Loads all data from scene space
            JArray spaceArray = (JArray)jsonObject[scene];

            // if data should be deleted, deletes it,
            // othervise add or update it
            if (data.GetPermissionForDelete())
            {
                DeleteDataByKey(spaceArray, data.GetSaveKey());
            }
            else
            {
                SaveOrUpdateData(spaceArray, data);
            }

            // save json object to file

            string stringToSave = jsonObject.ToString(Formatting.Indented);

            // encrypt if needed
            if (_useEncryption) stringToSave = EncryptDecrypt(stringToSave);

            // write
            using (StreamWriter writer = new StreamWriter(_savePath))
            {
                writer.Write(stringToSave);
            }

            callback?.Invoke(true);
        }

        /// <summary>
        /// Checks if data already exists in save file. If exists, updates it,
        /// othervise adds new one
        /// </summary>
        private static void SaveOrUpdateData(JArray spaceArray, T data)
        {
            bool itemExists = false;

            foreach (var item in spaceArray)
            {
                var existingItem = item.ToObject<T>();

                if (existingItem.GetSaveKey() == data.GetSaveKey())
                {
                    // update existing data
                    item.Replace(JObject.FromObject(data));
                    itemExists = true;
                    break;
                }
            }

            // if no any, creates new
            if (!itemExists)
            {
                spaceArray.Add(JObject.FromObject(data));
            }
        }

        /// <summary>
        /// Loads all data containers from scene space
        /// </summary>
        public static List<T> LoadAllData(string scene)
        {
            scene = ParseSceneName(scene);

            List<T> data = new List<T>();

            JObject jsonObject = TryLoadSaveFile();

            if (jsonObject[scene] != null)
            {
                JArray spaceArray = (JArray)jsonObject[scene];

                foreach (var item in spaceArray)
                {
                    T itemObject = item.ToObject<T>();
                    data.Add(itemObject);
                }
            }
            else
            {
#if DEBUG_MODE_SAVELOAD
    Debug.LogError(($"Space \"{space}\" cannot be found));
#endif
            }

            return data;
        }

        /// <summary>
        /// Loads data container by scene name and save key
        /// </summary>
        public static T LoadById(string scene, string saveKey)
        {
            scene = ParseSceneName(scene);

            T result = default;

            JObject jsonObject = TryLoadSaveFile();

            if (jsonObject[scene] != null)
            {
                JArray spaceArray = (JArray)jsonObject[scene];

                // looks for object by save key
                foreach (JToken item in spaceArray)
                {
                    T itemObject = item.ToObject<T>();

                    if (itemObject.GetSaveKey() != saveKey) continue;

                    result = itemObject;
                    break;
                }
            }
            else
            {
#if DEBUG_MODE_SAVELOAD
    Debug.LogError(($"Space \"{space}\" cannot be found));
#endif
            }

            return result;
        }

        /// <summary>
        /// Deletes data in the given array space by its save key
        /// </summary>
        public static void DeleteDataByKey(JArray spaceArray, string saveKey)
        {
            for (int i = 0; i < spaceArray.Count; i++)
            {
                var item = spaceArray[i].ToObject<T>();

                if (item.GetSaveKey() == saveKey)
                {
                    spaceArray.RemoveAt(i);
#if DEBUG_MODE_SAVELOAD
    Debug.Log(($"Object with key \"{saveKey}\" deleted sucessfuly"));
#endif
                    break;
                }
            }
        }

        /// <summary>
        /// Deletes data in the specific scene space by its save key
        /// </summary>
        public static void DeleteDataById(string scene, string saveKey)
        {
            scene = ParseSceneName(scene);

            JObject jsonObject = TryLoadSaveFile();

            if (jsonObject[scene] != null)
            {
                JArray spaceArray = (JArray)jsonObject[scene];

                for (int i = 0; i < spaceArray.Count; i++)
                {
                    var item = spaceArray[i].ToObject<T>();
                    if (item.GetSaveKey() == saveKey)
                    {
                        spaceArray.RemoveAt(i);
#if DEBUG_MODE_SAVELOAD
    Debug.Log(($"Object with key \"{saveKey}\" deleted sucessfuly"));
#endif
                        break;
                    }
                }

                File.WriteAllText(_savePath, jsonObject.ToString(Formatting.Indented));
            }
            else
            {
#if DEBUG_MODE_SAVELOAD
    Debug.LogError(($"Space \"{space}\" cannot be found));
#endif
            }
        }

        /// <summary>
        /// Cleanses all data
        /// </summary>
        public static void ClearSpace(string scene)
        {
            scene = ParseSceneName(scene);

            JObject jsonObject = TryLoadSaveFile();

            if (jsonObject[scene] != null)
            {
                // deletes scene space
                jsonObject.Remove(scene);

                // save json file
                //_streamReader = _streamReader is null ? new StreamReader(_savePath) : _streamReader
                using (var writer = new StreamWriter(_savePath))
                {
                    writer.Write(jsonObject.ToString(Formatting.Indented));
                }

#if DEBUG_MODE_SAVELOAD
    Debug.Log(($"Objects in space \"{scene}\" deleted sucessfuly"));
#endif
            }
            else
            {
#if DEBUG_MODE_SAVELOAD
    Debug.LogError(($"Space \"{space}\" cannot be found));
#endif
            }
        }

        public static List<string> GetFilePathsInFolder(string folderPath)
        {
            List<string> filePaths = new List<string>();

            if (Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath);
                filePaths.AddRange(files);
            }
            else
            {
                Debug.LogError("Folder does not exist: " + folderPath);
            }

            return filePaths;
        }

        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }

        /// <summary>
        /// Encrypt normal string, decrypt encrypted string
        /// </summary>
        private static string EncryptDecrypt(string data)
        {
            string modifiedData = "";
            for (int i = 0; i < data.Length; i++)
            {
                modifiedData += (char)(data[i] ^ _encryptingCodeWord[i % _encryptingCodeWord.Length]);
            }
            return modifiedData;
        }

        public static void InstantiateAllObjects(string scenePath, bool ignorePlayer)
        {
            List<T> savedData = LoadAllData(scenePath);

            foreach (var saveData in savedData)
            {
                if (saveData.GetPermissionForDelete()) continue;

                //if (ignorePlayer && saveData.IsPlayer()) continue; // Assuming there is an IsPlayer method or similar logic

                GameObject go = GameObject.Instantiate(LoadAssetUtility.Load<GameObject>(saveData.GetPrefabPath()));
                //go.GetComponent<ICanLoadAndSave<SLDataContainer>>().LoadData(saveData.GetSaveKey());

                if (go.TryGetComponent(out ICanSaveAndLoadDataContainer saveScript))
                {
                    saveScript.LoadData(saveData.GetSaveKey(), () => go.SetActive(true));

                    //saveScript.SLDataContainer.LoadContainer<SLDataContainer>();

                    //go.SetActive(true);
                }
                else
                {
                    Debug.Log(($"This life fucked you again"));
                }
            }
        }

        /// <summary>
        /// Extracts scene name from scene path
        /// </summary>
        private static string ParseSceneName(string scenePathName)
        {
            return scenePathName.Split("/").Last().Split(".")[0];
        }

        /// <summary>
        /// Loads json object from file located in save file folder
        /// </summary>
        /// <returns>Save JObject if successfully, empty if not</returns>
        private static JObject TryLoadSaveFile()
        {
            if (File.Exists(_savePath) && new FileInfo(_savePath).Length != 0)
            {
                //_streamReader = _streamReader is null ? new StreamReader(_savePath) : _streamReader
                using (var reader = new StreamReader(_savePath))
                {
                    string existingJson = reader.ReadToEnd();

                    if (_useEncryption) existingJson = EncryptDecrypt(existingJson);

                    return JObject.Parse(existingJson);
                }
            }
            else
            {
#if DEBUG_MODE_SAVELOAD
    Debug.LogError(($"Cannot find save file on path: {_savePath}"));
#endif
                return new JObject();
            }
        }
    }
}