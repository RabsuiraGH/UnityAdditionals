namespace Core.Utility.SaveLoad
{
    public interface IData
    {
        public string SaveKey { get; }

        public string GetSaveKey();

        public string GetScenePath();

        public string GetPrefabPath();

        public bool GetPermissionForDelete();
    }
}