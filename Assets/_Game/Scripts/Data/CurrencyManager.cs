using System;
using UnityEngine;
using WheelMasterRun.Core;

namespace WheelMasterRun.Data
{
    public class CurrencyManager : MonoBehaviour
    {
        public static CurrencyManager Instance { get; private set; }

        public event Action OnCurrencyUpdated;

        private GameData activeData;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadData();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadData()
        {
            activeData = SaveSystem.LoadGameData();
        }

        public int Coins => activeData != null ? activeData.coins : 0;
        public int Gems => activeData != null ? activeData.gems : 0;
        public string SelectedBike => activeData != null ? activeData.selectedBike : "BMX";

        public void AddCoins(int amount)
        {
            if (amount <= 0) return;
            activeData.coins += amount;
            SaveSystem.SaveGameData(activeData);
            OnCurrencyUpdated?.Invoke();
        }

        public bool TrySpendCoins(int amount)
        {
            if (amount <= 0) return false;
            if (activeData.coins >= amount)
            {
                activeData.coins -= amount;
                SaveSystem.SaveGameData(activeData);
                OnCurrencyUpdated?.Invoke();
                return true;
            }
            return false;
        }

        public void AddGems(int amount)
        {
            if (amount <= 0) return;
            activeData.gems += amount;
            SaveSystem.SaveGameData(activeData);
            OnCurrencyUpdated?.Invoke();
        }

        public bool TrySpendGems(int amount)
        {
            if (amount <= 0) return false;
            if (activeData.gems >= amount)
            {
                activeData.gems -= amount;
                SaveSystem.SaveGameData(activeData);
                OnCurrencyUpdated?.Invoke();
                return true;
            }
            return false;
        }

        public bool IsBikeUnlocked(string bikeID)
        {
            return activeData.unlockedBikes.Contains(bikeID);
        }

        public bool UnlockBike(string bikeID, int cost)
        {
            if (IsBikeUnlocked(bikeID)) return true;
            if (TrySpendCoins(cost))
            {
                activeData.unlockedBikes.Add(bikeID);
                SaveSystem.SaveGameData(activeData);
                OnCurrencyUpdated?.Invoke();
                return true;
            }
            return false;
        }

        public void SelectBike(string bikeID)
        {
            if (IsBikeUnlocked(bikeID))
            {
                activeData.selectedBike = bikeID;
                SaveSystem.SaveGameData(activeData);
                OnCurrencyUpdated?.Invoke();
            }
        }

        public int GetCurrentLevel()
        {
            return activeData != null ? activeData.currentLevel : 1;
        }

        public void AdvanceLevel()
        {
            activeData.currentLevel++;
            SaveSystem.SaveGameData(activeData);
            OnCurrencyUpdated?.Invoke();
        }
    }
}
