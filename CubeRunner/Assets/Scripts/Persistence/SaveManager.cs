using UnityEngine;

namespace Assets.Scripts.Persistence
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; set; }
        public SaveState state;

        private const string saveKey = "save";

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            Load();

            Debug.Log(state.Serialize<SaveState>());
        }

        public void Save()
        {
            PlayerPrefs.SetString(saveKey, state.Serialize<SaveState>());
        }

        public void Load()
        {
            if (PlayerPrefs.HasKey(saveKey))
            {
                state = PlayerPrefs.GetString(saveKey).Deserialize<SaveState>();
            }
            else
            {
                state = new SaveState();
                Save();
                Debug.Log("No save file found creating a new one");
            }
        }

    }
}