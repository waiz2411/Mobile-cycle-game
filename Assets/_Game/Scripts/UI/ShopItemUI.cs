using UnityEngine;
using UnityEngine.UI;
using WheelMasterRun.Data;

namespace WheelMasterRun.UI
{
    public class ShopItemUI : MonoBehaviour
    {
        [Header("UI Fields")]
        [SerializeField] private Text bikeNameText;
        [SerializeField] private Image bikeIcon;
        [SerializeField] private Text statusText; // "Buy 500", "Select", "Equipped"
        [SerializeField] private Button actionButton;

        private BikeConfig currentConfig;
        private ShopUI parentShop;

        public void SetupItem(BikeConfig config, ShopUI shop)
        {
            currentConfig = config;
            parentShop = shop;

            if (bikeNameText != null) bikeNameText.text = config.bikeName;
            if (bikeIcon != null) bikeIcon.sprite = config.bikeIcon;

            actionButton.onClick.AddListener(OnItemClicked);
            RefreshItemState();
        }

        public void RefreshItemState()
        {
            if (currentConfig == null || CurrencyManager.Instance == null) return;

            string id = currentConfig.bikeID;

            if (CurrencyManager.Instance.SelectedBike == id)
            {
                if (statusText != null) statusText.text = "EQUIPPED";
                if (actionButton != null) actionButton.interactable = false;
            }
            else if (CurrencyManager.Instance.IsBikeUnlocked(id))
            {
                if (statusText != null) statusText.text = "SELECT";
                if (actionButton != null) actionButton.interactable = true;
            }
            else
            {
                if (statusText != null) statusText.text = $"{currentConfig.coinUnlockCost} COINS";
                if (actionButton != null) actionButton.interactable = CurrencyManager.Instance.Coins >= currentConfig.coinUnlockCost;
            }
        }

        private void OnItemClicked()
        {
            if (parentShop != null && currentConfig != null)
            {
                parentShop.TryPurchaseOrSelect(currentConfig);
            }
        }
    }
}
