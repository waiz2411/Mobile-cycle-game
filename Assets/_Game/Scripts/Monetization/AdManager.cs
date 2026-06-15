using System;
using UnityEngine;

namespace WheelMasterRun.Monetization
{
    public class AdManager : MonoBehaviour
    {
        public static AdManager Instance { get; private set; }

        [Header("AdMob Settings (Simulated for Editor compatibility)")]
        [SerializeField] private string androidAppId = "ca-app-pub-3940256099942544~3347511713";
        [SerializeField] private string iosAppId = "ca-app-pub-3940256099942544~1458002511";
        [SerializeField] private string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
        [SerializeField] private string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";

        private Action onRewardedAdCompleted;
        private Action onRewardedAdFailed;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAds();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAds()
        {
            Debug.Log("Initializing Ads (AdMob Ready)...");
            // Production AdMob: MobileAds.Initialize((initStatus) => { LoadAds(); });
        }

        public void ShowInterstitialAd()
        {
            Debug.Log("Showing Interstitial Ad...");
            // Simulate Ad display in Editor
            #if UNITY_EDITOR
            UIManagerSimulateMessage("AdMob Interstitial Ad Shown");
            #endif
        }

        public void ShowRewardedAd(Action onSuccess, Action onFailure)
        {
            onRewardedAdCompleted = onSuccess;
            onRewardedAdFailed = onFailure;

            Debug.Log("Showing Rewarded Ad...");

            #if UNITY_EDITOR
            // Auto complete in editor after 1 second for fast testing
            Invoke(nameof(SimulateRewardedCompletion), 1f);
            #else
            // Production AdMob integration:
            // if (rewardedAd != null && rewardedAd.CanShowAd()) {
            //     rewardedAd.Show((Reward reward) => { onRewardedAdCompleted?.Invoke(); });
            // } else {
            //     onRewardedAdFailed?.Invoke();
            // }
            SimulateRewardedCompletion(); // Fallback simulation
            #endif
        }

        private void SimulateRewardedCompletion()
        {
            onRewardedAdCompleted?.Invoke();
            Debug.Log("Rewarded Ad completed successfully!");
        }

        #if UNITY_EDITOR
        private void UIManagerSimulateMessage(string message)
        {
            Debug.Log($"[Simulated Ad UI]: {message}");
        }
        #endif
    }
}
