using UnityEngine;
using UnityEngine.UI;
using WheelMasterRun.Data;

namespace WheelMasterRun.UI
{
    public class ShopUI : MonoBehaviour
    {
        [Header("Bike List Configuration")]
        [SerializeField] private BikeConfig[] availableBikes;
        [SerializeField] private GameObject shopItemPrefab;
        [SerializeField] private Transform contentContainer;

        private void Start()
        {
            PopulateShop();
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnCurrencyUpdated += UpdateShopUI;
            }
        }

        private void PopulateShop()
        {
            // Clear existing elements in content container
            foreach (Transform child in contentContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (BikeConfig config in availableBikes)
            {
                GameObject itemObj = Instantiate(shopItemPrefab, contentContainer);
                ShopItemUI itemUI = itemObj.GetComponent<ShopItemUI>();
                if (itemUI != null)
                {
                    itemUI.SetupItem(config, this);
                }
            }
        }

        public void UpdateShopUI()
        {
            foreach (Transform child in contentContainer)
            {
                ShopItemUI itemUI = child.GetComponent<ShopItemUI>();
                if (itemUI != null)
                {
                    itemUI.RefreshItemState();
                }
            }
        }

        public void TryPurchaseOrSelect(BikeConfig config)
        {
            if (CurrencyManager.Instance == null) return;

            string id = config.bikeID;
            if (CurrencyManager.Instance.IsBikeUnlocked(id))
            {
                CurrencyManager.Instance.SelectBike(id);
            }
            else
            {
                if (CurrencyManager.Instance.UnlockBike(id, config.coinUnlockCost))
                {
                    CurrencyManager.Instance.SelectBike(id);
                }
                else
                {
                    Debug.Log("Insufficient Coins!");
                }
            }
            UpdateShopUI();
        }
    }
}
