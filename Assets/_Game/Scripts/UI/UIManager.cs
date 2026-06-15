using UnityEngine;
using UnityEngine.UI;
using WheelMasterRun.Core;
using WheelMasterRun.Data;

namespace WheelMasterRun.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private GameObject shopPanel;

        [Header("HUD Elements")]
        [SerializeField] private Text levelTextHUD;
        [SerializeField] private Text coinsTextHUD;
        [SerializeField] private Text gemsTextHUD;
        [SerializeField] private Slider levelProgressBar;

        [Header("Victory/GameOver Screens")]
        [SerializeField] private Text rewardGemsText;
        [SerializeField] private Text multiplierText;

        [Header("Progress Track Target")]
        private Transform playerTransform;
        private Transform finishTransform;
        private float startDistanceZ;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += UpdateUIState;
                UpdateUIState(GameManager.Instance.CurrentState);
            }

            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnCurrencyUpdated += RefreshCurrencyUI;
            }

            RefreshCurrencyUI();
        }

        private void Update()
        {
            UpdateProgressBar();
        }

        private void UpdateUIState(GameState state)
        {
            menuPanel.SetActive(state == GameState.Menu);
            hudPanel.SetActive(state == GameState.Playing);
            gameOverPanel.SetActive(state == GameState.GameOver);
            victoryPanel.SetActive(state == GameState.Victory);
            shopPanel.SetActive(state == GameState.Shop);

            if (state == GameState.Playing)
            {
                FindTrackingTargets();
            }
        }

        private void FindTrackingTargets()
        {
            // Dynamically locate player and finish line in the scene
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }

            GameObject finishObj = GameObject.FindWithTag("Finish");
            if (finishObj != null)
            {
                finishTransform = finishObj.transform;
            }

            if (playerTransform != null && finishTransform != null)
            {
                startDistanceZ = finishTransform.position.z - playerTransform.position.z;
            }
        }

        private void UpdateProgressBar()
        {
            if (playerTransform == null || finishTransform == null || startDistanceZ <= 0) return;

            float currentDistanceZ = finishTransform.position.z - playerTransform.position.z;
            float progress = 1f - (currentDistanceZ / startDistanceZ);
            
            if (levelProgressBar != null)
            {
                levelProgressBar.value = Mathf.Clamp01(progress);
            }
        }

        public void RefreshCurrencyUI()
        {
            if (CurrencyManager.Instance == null) return;

            int level = CurrencyManager.Instance.GetCurrentLevel();
            int coins = CurrencyManager.Instance.Coins;
            int gems = CurrencyManager.Instance.Gems;

            if (levelTextHUD != null) levelTextHUD.text = $"LEVEL {level}";
            if (coinsTextHUD != null) coinsTextHUD.text = coins.ToString();
            if (gemsTextHUD != null) gemsTextHUD.text = gems.ToString();
        }

        public void SetupVictoryScreen(int rewardGems, int multiplier)
        {
            if (rewardGemsText != null) rewardGemsText.text = $"+{rewardGems} GEMS";
            if (multiplierText != null) multiplierText.text = $"{multiplier}X MULTIPLIER";
        }

        // Button events
        public void OnPlayButtonPressed()
        {
            GameManager.Instance.StartGame();
        }

        public void OnShopButtonPressed()
        {
            GameManager.Instance.OpenShop();
        }

        public void OnCloseShopButtonPressed()
        {
            GameManager.Instance.CloseShop();
        }

        public void OnRestartButtonPressed()
        {
            GameManager.Instance.RestartLevel();
        }

        public void OnNextLevelButtonPressed()
        {
            GameManager.Instance.NextLevel();
        }

        public void OnContinueAdButtonPressed()
        {
            GameManager.Instance.ContinueAfterAd();
        }
    }
}
