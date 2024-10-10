namespace Core.Utility.JsonSaver
{
    public interface IJsonData
    {
        public string SaveKey { get; }

        public string GetSaveKey();

        public bool GetPermissionForDelete();
    }
}