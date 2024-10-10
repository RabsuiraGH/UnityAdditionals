using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Utility.SaveLoad
{
    public class Loader : MonoBehaviour
    {
        [field: SerializeField] public string CurrentScene { get; set; }

        private void Awake()
        {
            CurrentScene = SceneManager.GetActiveScene().path;
        }

        [ContextMenu(nameof(Load))]
        public void Load()
        {
            SaveLoadUtility<SLDataContainer>.InstantiateAllObjects(CurrentScene, true);
        }
    }
}