using System;

namespace Core.Utility.SaveLoad
{
    public interface ICanSaveAndLoadDataContainer
    {
        public bool DontSave { get; }
        public void LoadData(string saveKey, Action onLoad);

        public void SaveData();
    }
}