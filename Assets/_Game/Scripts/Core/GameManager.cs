using UnityEngine;
using UnityEngine.SceneManagement;
using WheelMasterRun.Environment;
using WheelMasterRun.Player;
using WheelMasterRun.Data;
using WheelMasterRun.Monetization;

namespace WheelMasterRun.Core
{
    public enum GameState { Menu, Playing, GameOver, Victory, Shop }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; } = GameState.Menu;

        [Header("References")]
        [SerializeField] private PlayerController playerPrefab;
        [SerializeField] private Transform startPoint;

        private PlayerController spawnedPlayer;
        private int adCounter = 0;

        public event System.Action<GameState> OnStateChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SetState(GameState.Menu);
            LoadLevelEnvironment();
        }

        public void SetState(GameState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);

            switch (newState)
            {
                case GameState.Menu:
                    Time.timeScale = 1f;
                    break;
                case GameState.Playing:
                    Time.timeScale = 1f;
                    if (spawnedPlayer != null)
                    {
                        spawnedPlayer.StartRun();
                    }
                    break;
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    if (spawnedPlayer != null) spawnedPlayer.StopRun();
                    break;
                case GameState.Victory:
                    Time.timeScale = 0f;
                    if (spawnedPlayer != null) spawnedPlayer.StopRun();
                    break;
                case GameState.Shop:
                    Time.timeScale = 1f;
                    break;
            }
        }

        public void LoadLevelEnvironment()
        {
            // Spawn Player
            if (spawnedPlayer != null) Destroy(spawnedPlayer.gameObject);
            
            Vector3 spawnPos = startPoint != null ? startPoint.position : Vector3.zero;
            spawnedPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

            // Register size loss
            WheelSystem wheels = spawnedPlayer.GetComponent<WheelSystem>();
            if (wheels != null)
            {
                wheels.OnPlayerLost += HandlePlayerLost;
            }

            // Generate Map
            int currentLevel = CurrencyManager.Instance != null ? CurrencyManager.Instance.GetCurrentLevel() : 1;
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.GenerateLevel(currentLevel);
            }
        }

        private void HandlePlayerLost()
        {
            SetState(GameState.GameOver);
        }

        public void StartGame()
        {
            SetState(GameState.Playing);
        }

        public void OpenShop()
        {
            SetState(GameState.Shop);
        }

        public void CloseShop()
        {
            SetState(GameState.Menu);
        }

        public void CompleteLevel(int gemReward, int multiplier)
        {
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddGems(gemReward);
                CurrencyManager.Instance.AdvanceLevel();
            }

            SetState(GameState.Victory);

            // Ad counter logic
            adCounter++;
            if (adCounter % 2 == 0 && AdManager.Instance != null)
            {
                AdManager.Instance.ShowInterstitialAd();
            }
        }

        public void RestartLevel()
        {
            SetState(GameState.Menu);
            LoadLevelEnvironment();
        }

        public void NextLevel()
        {
            SetState(GameState.Menu);
            LoadLevelEnvironment();
        }

        public void ContinueAfterAd()
        {
            if (AdManager.Instance != null)
            {
                AdManager.Instance.ShowRewardedAd(
                    onSuccess: () => {
                        // Restore player size and continue
                        if (spawnedPlayer != null)
                        {
                            WheelSystem wheels = spawnedPlayer.GetComponent<WheelSystem>();
                            if (wheels != null)
                            {
                                wheels.AddWheelSize(1.5f); // Boost size to continue safely
                            }
                            SetState(GameState.Playing);
                        }
                    },
                    onFailure: () => {
                        Debug.Log("Rewarded ad failed. Cannot continue.");
                    }
                );
            }
        }
    }
}
