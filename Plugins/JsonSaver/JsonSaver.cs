using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Utility.JsonSaver
{
    public static class JsonSaver<T> where T : IJsonData
    {
        private static readonly string _encryptingCodeWord = "MyBaseEncryptionCode";

        /// <summary>
        /// Save data container in specific space
        /// </summary>
        ///
        public static void SaveData(string fullSavePath, string space, T data, Action<bool> callback = null, bool encryption = false)
        {
            // parses name from path and loads full jobjec
            JObject jsonObject = TryLoadFile(fullSavePath, encryption);

            // if no space space in save, creates new one
            if (jsonObject[space] == null)
            {
                jsonObject[space] = new JArray();
            }

            // Loads all data from space space
            JArray spaceArray = (JArray)jsonObject[space];

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
            if (encryption) stringToSave = EncryptDecrypt(stringToSave);

            // write
            using (StreamWriter writer = new StreamWriter(fullSavePath))
            {
                writer.Write(stringToSave);
            }

            callback?.Invoke(true);
            Console.WriteLine($"Data saved to {fullSavePath}");
        }

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
        /// Loads all data containers from space
        /// </summary>
        public static List<T> LoadAllData(string fullSavePath, string space, bool encryption = false)
        {
            List<T> data = new List<T>();

            JObject jsonObject = TryLoadFile(fullSavePath, encryption);

            if (jsonObject[space] != null)
            {
                JArray spaceArray = (JArray)jsonObject[space];

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
        /// Loads data container by space name and save key
        /// </summary>
        public static T LoadById(string space, string saveKey, string fullSavePath, bool encryption = false)
        {
            T result = default;

            JObject jsonObject = TryLoadFile(fullSavePath, encryption);

            if (jsonObject[space] != null)
            {
                JArray spaceArray = (JArray)jsonObject[space];

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

        private static JObject TryLoadFile(string fullSavePath, bool encryption)
        {
            if (File.Exists(fullSavePath) && new FileInfo(fullSavePath).Length != 0)
            {
                using (var reader = new StreamReader(fullSavePath))
                {
                    string existingJson = reader.ReadToEnd();

                    if (encryption) existingJson = EncryptDecrypt(existingJson);

                    return JObject.Parse(existingJson);
                }
            }
            else
            {
#if DEBUG_MODE_SAVELOAD
    Debug.LogError(($"Cannot find save file on path: {_fullSavePath}"));
#endif
                return new JObject();
            }
        }
    }
}