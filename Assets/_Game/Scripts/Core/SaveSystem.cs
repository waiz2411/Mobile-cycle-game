using UnityEngine;

namespace WheelMasterRun.Core
{
    public static class SaveSystem
    {
        private const string SAVE_KEY = "WheelMasterRun_SaveData";

        public static GameData LoadGameData()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                try
                {
                    GameData data = JsonUtility.FromJson<GameData>(json);
                    if (data != null)
                    {
                        return data;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error parsing save data: {e.Message}");
                }
            }
            
            // Return fresh data if none exists or loading failed
            return new GameData();
        }

        public static void SaveGameData(GameData data)
        {
            if (data == null) return;
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }
    }
}
